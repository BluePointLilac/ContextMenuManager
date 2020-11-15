using BulePointLilac.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class NewSendToDialog : CommonDialog
    {
        public string FilePath { get; private set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(NewSendToItemForm frm = new NewSendToItemForm())
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.FilePath = frm.FilePath;
                return flag;
            }
        }

        sealed class NewSendToItemForm : NewItemForm
        {
            public string FilePath { get; set; }

            readonly RadioButton rdoFile = new RadioButton
            {
                Text = AppString.SideBar.File,
                AutoSize = true,
                Checked = true
            };
            readonly RadioButton rdoFolder = new RadioButton
            {
                Text = AppString.SideBar.Folder,
                AutoSize = true
            };

            protected override void InitializeComponents()
            {
                base.InitializeComponents();
                this.Text = AppString.Dialog.NewSendToItem;
                this.Controls.AddRange(new[] { rdoFile, rdoFolder });
                rdoFile.Top = rdoFolder.Top = btnOk.Top;
                rdoFile.Left = lblCommand.Left;
                rdoFolder.Left = rdoFile.Right + 20.DpiZoom();

                btnBrowse.Click += (sender, e) =>
                {
                    if(rdoFile.Checked) BrowseFile();
                    else BrowseFolder();
                };

                btnOk.Click += (sender, e) =>
                {
                    if(ItemText.IsNullOrWhiteSpace())
                    {
                        MessageBoxEx.Show(AppString.MessageBox.TextCannotBeEmpty);
                        return;
                    }
                    if(ItemCommand.IsNullOrWhiteSpace())
                    {
                        MessageBoxEx.Show(AppString.MessageBox.CommandCannotBeEmpty);
                        return;
                    }
                    if(ObjectPath.ExtractFilePath(ItemCommand) == null && !Directory.Exists(ItemCommand))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.FileOrFolderNotExists);
                        return;
                    }
                    AddNewItem();
                    DialogResult = DialogResult.OK;
                };
            }

            private void BrowseFile()
            {
                using(OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = $"{AppString.Dialog.Program}|*.exe;*.bat;*.cmd;*.vbs;*.vbe;*.jse;*.wsf";
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        ItemCommand = $"\"{dlg.FileName}\"";
                        ItemText = Path.GetFileNameWithoutExtension(dlg.FileName);
                    }
                }
            }

            private void BrowseFolder()
            {
                using(FolderBrowserDialog dlg = new FolderBrowserDialog())
                {
                    dlg.SelectedPath = ItemCommand;
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        ItemCommand = dlg.SelectedPath;
                        ItemText = new DirectoryInfo(dlg.SelectedPath).Name;
                    }
                }
            }

            private void AddNewItem()
            {
                FilePath = $@"{SendToList.SendToPath}\{ObjectPath.RemoveIllegalChars(ItemText)}.lnk";
                FilePath = ObjectPath.GetNewPathWithIndex(FilePath, ObjectPath.PathType.File);

                WshShortcut shortcut = new WshShortcut { FullName = FilePath };
                if(rdoFile.Checked)
                {
                    ItemCommand = Environment.ExpandEnvironmentVariables(ItemCommand);
                    shortcut.TargetPath = ObjectPath.ExtractFilePath(ItemCommand, out string shortPath);
                    string str = ItemCommand.Substring(ItemCommand.IndexOf(shortPath) + shortPath.Length);
                    shortcut.Arguments = str.Substring(str.IndexOf(" ") + 1);
                    shortcut.WorkingDirectory = Path.GetDirectoryName(shortcut.TargetPath);
                }
                else shortcut.TargetPath = ItemCommand;
                shortcut.Save();
                DesktopIniHelper.SetLocalizedFileName(FilePath, ItemText);
            }
        }
    }
}