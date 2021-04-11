using BluePointLilac.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class NewLnkFileDialog : CommonDialog
    {
        public string ItemText { get; set; }
        public string ItemFilePath { get; set; }
        public string Arguments { get; set; }
        public string FileFilter { get; set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(NewLnkForm frm = new NewLnkForm())
            {
                frm.FileFilter = this.FileFilter;
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag)
                {
                    this.ItemText = frm.ItemText;
                    this.ItemFilePath = frm.ItemFilePath;
                    this.Arguments = frm.Arguments;
                }
                return flag;
            }
        }

        sealed class NewLnkForm : NewItemForm
        {
            public string FileFilter { get; set; }

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
                    }
                    else if(ItemFilePath.IsNullOrWhiteSpace())
                    {
                        MessageBoxEx.Show(AppString.MessageBox.CommandCannotBeEmpty);
                    }
                    else if(rdoFile.Checked && !ObjectPath.GetFullFilePath(ItemFilePath, out _))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.FileNotExists);
                    }
                    else if(rdoFolder.Checked && !Directory.Exists(ItemFilePath))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.FolderNotExists);
                    }
                    else DialogResult = DialogResult.OK;
                };

                txtFilePath.TextChanged += (sender, e) =>
                {
                    if(Path.GetExtension(ItemFilePath).ToLower() == ".lnk")
                    {
                        using(ShellLink shortcut = new ShellLink(ItemFilePath))
                        {
                            if(File.Exists(shortcut.TargetPath))
                            {
                                ItemFilePath = shortcut.TargetPath;
                            }
                        }
                    }
                };
            }

            private void BrowseFile()
            {
                using(OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = this.FileFilter;
                    //取消获取lnk目标路径，可选中UWP快捷方式
                    dlg.DereferenceLinks = false;
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        ItemFilePath = dlg.FileName;
                        string extension = Path.GetExtension(dlg.FileName).ToLower();
                        if(extension == ".lnk")
                        {
                            using(ShellLink shortcut = new ShellLink(dlg.FileName))
                            {
                                if(File.Exists(shortcut.TargetPath))
                                {
                                    ItemFilePath = shortcut.TargetPath;
                                    Arguments = shortcut.Arguments;
                                }
                            }
                        }
                        ItemText = Path.GetFileNameWithoutExtension(dlg.FileName);
                    }
                }
            }

            private void BrowseFolder()
            {
                using(FolderBrowserDialog dlg = new FolderBrowserDialog())
                {
                    if(Directory.Exists(ItemFilePath)) dlg.SelectedPath = ItemFilePath;
                    else dlg.SelectedPath = Application.StartupPath;
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        ItemFilePath = dlg.SelectedPath;
                        ItemText = Path.GetFileNameWithoutExtension(dlg.SelectedPath);
                    }
                }
            }
        }
    }
}