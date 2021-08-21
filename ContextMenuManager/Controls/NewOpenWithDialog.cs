using BluePointLilac.Methods;
using ContextMenuManager.Methods;
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
                frm.TopMost = AppConfig.TopMost;
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
            private string AppRegPath => $@"{RegistryEx.CLASSES_ROOT}\Applications\{FileName}";
            private string CommandPath => $@"{AppRegPath}\shell\open\command";

            protected override void InitializeComponents()
            {
                base.InitializeComponents();
                btnBrowse.Click += (sender, e) => BrowseFile();
                btnOK.Click += (sender, e) =>
                {
                    if(string.IsNullOrEmpty(ItemText))
                    {
                        AppMessageBox.Show(AppString.Message.TextCannotBeEmpty);
                        return;
                    }
                    if(ItemCommand.IsNullOrWhiteSpace())
                    {
                        AppMessageBox.Show(AppString.Message.CommandCannotBeEmpty);
                        return;
                    }
                    FilePath = ObjectPath.ExtractFilePath(base.ItemFilePath);
                    using(var key = RegistryEx.GetRegistryKey(CommandPath))
                    {
                        string path = ObjectPath.ExtractFilePath(key?.GetValue("")?.ToString());
                        string name = Path.GetFileName(path);
                        if(FilePath != null && FilePath.Equals(path, StringComparison.OrdinalIgnoreCase))
                        {
                            AppMessageBox.Show(AppString.Message.HasBeenAdded);
                            return;
                        }
                        if(FileName == null || FileName.Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            AppMessageBox.Show(AppString.Message.UnsupportedFilename);
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