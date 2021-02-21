using BluePointLilac.Methods;
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
            private string FileName => Path.GetFileName(FilePath);
            private string AppRegPath => $@"HKEY_CLASSES_ROOT\Applications\{FileName}";
            private string CommandPath => $@"{AppRegPath}\shell\open\command";

            protected override void InitializeComponents()
            {
                base.InitializeComponents();
                btnBrowse.Click += (sender, e) => BrowseFile();
                btnOk.Click += (sender, e) =>
                {
                    if(string.IsNullOrEmpty(ItemText))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.TextCannotBeEmpty);
                        return;
                    }
                    if(ItemCommand.IsNullOrWhiteSpace())
                    {
                        MessageBoxEx.Show(AppString.MessageBox.CommandCannotBeEmpty);
                        return;
                    }
                    FilePath = ObjectPath.ExtractFilePath(base.ItemFilePath);
                    using(var key = RegistryEx.GetRegistryKey(CommandPath))
                    {
                        string path = ObjectPath.ExtractFilePath(key?.GetValue("")?.ToString());
                        string name = Path.GetFileName(path);
                        if(FilePath != null && FilePath.Equals(path, StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBoxEx.Show(AppString.MessageBox.HasBeenAdded);
                            return;
                        }
                        if(FileName == null || FileName.Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBoxEx.Show(AppString.MessageBox.UnsupportedFilename);
                            return;
                        }
                    }
                    AddNewItem();
                    this.DialogResult = DialogResult.OK;
                };
            }

            private void BrowseFile()
            {
                using(OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = $"{AppString.Dialog.Program}|*.exe";
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        base.ItemFilePath = dlg.FileName;
                        Arguments = "\"%1\"";
                        ItemText = FileVersionInfo.GetVersionInfo(dlg.FileName).FileDescription;
                    }
                }
            }

            private void AddNewItem()
            {
                using(var key = RegistryEx.GetRegistryKey(AppRegPath, true, true))
                {
                    key.SetValue("FriendlyAppName", ItemText);
                }
                using(var cmdKey = RegistryEx.GetRegistryKey(CommandPath, true, true))
                {
                    cmdKey.SetValue("", ItemCommand);
                    RegPath = cmdKey.Name;
                }
            }
        }
    }
}