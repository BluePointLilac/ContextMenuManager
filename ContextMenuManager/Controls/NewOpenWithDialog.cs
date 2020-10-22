using BulePointLilac.Methods;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class NewOpenWithDialog : CommonDialog
    {
        public string RegPath { get; private set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(NewOpenWithForm frm = new NewOpenWithForm())
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.RegPath = frm.RegPath;
                return flag;
            }
        }

        sealed class NewOpenWithForm : NewItemForm
        {
            public string RegPath { get; private set; }

            private string FilePath;
            private string AppRegPath;

            protected override void InitializeComponents()
            {
                base.InitializeComponents();
                this.Text = AppString.Text.NewOpenWithItem;
                btnBrowse.Click += (sender, e) => BrowseFile();
                btnOk.Click += (sender, e) =>
                {
                    if(string.IsNullOrEmpty(ItemText))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.TextCannotBeEmpty);
                        return;
                    }
                    if(string.IsNullOrWhiteSpace(ItemCommand))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.TextCannotBeEmpty);
                        return;
                    }
                    FilePath = ObjectPath.ExtractFilePath(ItemCommand);
                    AppRegPath = $@"HKEY_CLASSES_ROOT\Applications\{Path.GetFileName(FilePath)}";
                    if(FilePath == null || RegistryEx.GetRegistryKey(AppRegPath) != null)
                    {
                        MessageBoxEx.Show(AppString.MessageBox.UnsupportedFilename);
                        return;
                    }
                    AddNewItem();
                    this.DialogResult = DialogResult.OK;
                };
            }

            private void BrowseFile()
            {
                using(OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = $"{AppString.Indirect.Programs}|*.exe";
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        ItemCommand = $"\"{dlg.FileName}\" \"%1\"";
                        ItemText = FileVersionInfo.GetVersionInfo(dlg.FileName).FileDescription;
                    }
                }
            }

            private void AddNewItem()
            {
                using(var key = RegistryEx.GetRegistryKey(AppRegPath, true, true))
                {
                    key.SetValue("FriendlyAppName", ItemText);
                    using(var cmdKey = key.CreateSubKey(@"shell\open\command", true))
                    {
                        cmdKey.SetValue("", ItemCommand);
                        RegPath = cmdKey.Name;
                    }
                }
            }
        }
    }
}