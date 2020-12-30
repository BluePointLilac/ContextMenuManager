using BulePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class FileExtensionDialog : CommonDialog
    {
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

        sealed class FileExtensionForm : Form
        {
            public FileExtensionForm()
            {
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                this.Text = AppString.Dialog.SelectExtension;
                this.Font = SystemFonts.MenuFont;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.MaximizeBox = this.MinimizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.CenterParent;
                InitializeComponents();
                LoadExtensions();
                btnOk.Click += (sender, e) =>
                {
                    int index = cmbExtension.Text.IndexOf('.');
                    if(index >= 0) this.Extension = cmbExtension.Text.Substring(index);
                    else this.Extension = $".{cmbExtension.Text}";
                };
            }

            public string Extension { get; private set; }

            readonly ComboBox cmbExtension = new ComboBox
            {
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                DropDownHeight = 294.DpiZoom(),
                ImeMode = ImeMode.Disable
            };
            readonly Button btnOk = new Button
            {
                DialogResult = DialogResult.OK,
                Text = AppString.Dialog.Ok,
                AutoSize = true
            };
            readonly Button btnCancel = new Button
            {
                DialogResult = DialogResult.Cancel,
                Text = AppString.Dialog.Cancel,
                AutoSize = true
            };

            private void InitializeComponents()
            {
                this.Controls.AddRange(new Control[] { cmbExtension, btnOk, btnCancel });
                int a = 20.DpiZoom();
                cmbExtension.Left = a;
                cmbExtension.Width = 85.DpiZoom();
                cmbExtension.Top = btnOk.Top = btnCancel.Top = a;
                btnOk.Left = cmbExtension.Right + a;
                btnCancel.Left = btnOk.Right + a;
                this.ClientSize = new Size(btnCancel.Right + a, btnCancel.Bottom + a);
            }

            private void LoadExtensions()
            {
                foreach(string extension in Microsoft.Win32.Registry.ClassesRoot.GetSubKeyNames())
                {
                    if(!extension.StartsWith(".")) continue;
                    cmbExtension.Items.Add(extension.Substring(1));
                }
            }
        }
    }
}