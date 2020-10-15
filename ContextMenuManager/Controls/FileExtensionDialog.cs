using BulePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class FileExtensionDialog : CommonDialog
    {
        const string FileExtsPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
        public string Extension { get; private set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(FileExtensionForm frm = new FileExtensionForm())
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.Extension = frm.Extension;
                return flag;
            }
        }

        public static string GetTypeName(string extension, bool includeUWP = true)
        {
            using(var root = Microsoft.Win32.Registry.ClassesRoot)
            {
                bool TypeNameExists(string typeName)
                {
                    if(!string.IsNullOrEmpty(typeName))
                        using(var typeKey = root.OpenSubKey(typeName))
                            if(typeKey != null) return true;
                    return false;
                }

                using(var extKey = root.OpenSubKey(extension))
                {
                    if(extKey == null) return null;
                    string defaultType = extKey.GetValue("")?.ToString();
                    using(var key = extKey.OpenSubKey("OpenWithProgids"))
                    {
                        if(key == null)
                        {
                            if(TypeNameExists(defaultType)) return defaultType;
                            else return null;
                        }
                        foreach(string valueName in key.GetValueNames())
                        {
                            if(!includeUWP && key.GetValueKind(valueName) != Microsoft.Win32.RegistryValueKind.String)
                            {
                                if(TypeNameExists(defaultType)) return defaultType;
                                continue;
                            }
                            if(TypeNameExists(valueName)) return valueName;
                        }
                    }
                }
            }
            return null;
        }

        sealed class FileExtensionForm : Form
        {
            public FileExtensionForm()
            {
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                this.Text = AppString.Text_SelectExtension;
                this.Font = SystemFonts.MenuFont;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.MaximizeBox = this.MinimizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.CenterParent;
                InitializeComponents();
                LoadExtensions();
                btnOk.Click += (sender, e) =>
                {
                    if(cmbExtension.Items.Contains(cmbExtension.Text))
                    {
                        this.Extension = $".{cmbExtension.Text}";
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBoxEx.Show(AppString.MessageBox_UnsupportedExtension);
                        cmbExtension.Focus();
                    }
                };
            }

            public string Extension { get; private set; }

            readonly ComboBox cmbExtension = new ComboBox
            {
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                DropDownHeight = 294.DpiZoom()
            };
            readonly Button btnOk = new Button
            {
                Text = AppString.Ok,
                AutoSize = true
            };
            readonly Button btnCancel = new Button
            {
                DialogResult = DialogResult.Cancel,
                Text = AppString.Cancel,
                AutoSize = true
            };

            private void InitializeComponents()
            {
                this.ClientSize = new Size(316, 110).DpiZoom();
                this.Controls.AddRange(new Control[] { cmbExtension, btnOk, btnCancel });
                int a = 20.DpiZoom();
                cmbExtension.Left = a;
                cmbExtension.Width = 85.DpiZoom();
                cmbExtension.Top = btnOk.Top = btnCancel.Top = 2 * a;
                btnOk.Left = cmbExtension.Right + a;
                btnCancel.Left = btnOk.Right + a;
            }

            private void LoadExtensions()
            {
                using(var extKey = RegistryEx.GetRegistryKey(FileExtsPath))
                {
                    if(extKey == null) return;
                    foreach(string extension in extKey.GetSubKeyNames())
                    {
                        if(!extension.StartsWith(".") || GetTypeName(extension) == null) continue;
                        cmbExtension.Items.Add(extension.Substring(1));
                    }
                }
            }
        }
    }
}