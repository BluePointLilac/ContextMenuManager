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
            using(NewSendToForm frm = new NewSendToForm())
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.FilePath = frm.FilePath;
                return flag;
            }
        }

        sealed class NewSendToForm : NewItemForm
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
                this.Controls.AddRange(new Control[] { rdoFile, rdoFolder });
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
                    if(Command.IsNullOrWhiteSpace())
                    {
                        MessageBoxEx.Show(AppString.MessageBox.CommandCannotBeEmpty);
                        return;
                    }
                    if(rdoFile.Checked && !ObjectPath.GetFullFilePath(Command, out _))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.FileNotExists);
                        return;
                    }
                    if(rdoFolder.Checked && !Directory.Exists(Command))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.FolderNotExists);
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
                        Command = dlg.FileName;
                        ItemText = Path.GetFileNameWithoutExtension(dlg.FileName);
                    }
                }
            }

            private void BrowseFolder()
            {
                using(FolderBrowserDialog dlg = new FolderBrowserDialog())
                {
                    if(Directory.Exists(Command)) dlg.SelectedPath = Command;
                    else dlg.SelectedPath = Application.StartupPath;
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        Command = dlg.SelectedPath;
                        ItemText = new DirectoryInfo(dlg.SelectedPath).Name;
                    }
                }
            }

            private void AddNewItem()
            {
                FilePath = $@"{SendToList.SendToPath}\{ObjectPath.RemoveIllegalChars(ItemText)}.lnk";
                FilePath = ObjectPath.GetNewPathWithIndex(FilePath, ObjectPath.PathType.File);
                WshShortcut shortcut = new WshShortcut
                {
                    FullName = FilePath,
                    TargetPath = Command,
                    WorkingDirectory = Path.GetDirectoryName(Command),
                    Arguments = Arguments
                };
                shortcut.Save();
                SendToList.DesktopIniWriter.SetValue("LocalizedFileNames", Path.GetFileName(FilePath), ItemText);
            }
        }
    }
}