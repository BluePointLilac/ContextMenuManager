using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class SendToList : MyList
    {
        private static readonly string SendToPath = Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\SendTo");
        private static readonly string DefaultSendToPath = Environment.ExpandEnvironmentVariables(@"%SystemDrive%\Users\Default\AppData\Roaming\Microsoft\Windows\SendTo");

        public void LoadItems()
        {
            foreach(string path in Directory.GetFileSystemEntries(SendToPath))
            {
                if(Path.GetFileName(path).ToLower() == "desktop.ini") continue;
                this.AddItem(new SendToItem(path));
            }
            this.SortItemByText();
            this.AddNewItem();
            this.AddDirItem();
            this.AddItem(new VisibleRegRuleItem(VisibleRegRuleItem.SendToDrive));
            this.AddItem(new VisibleRegRuleItem(VisibleRegRuleItem.DeferBuildSendTo));
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem();
            this.InsertItem(newItem, 0);
            newItem.AddNewItem += () =>
            {
                using(NewLnkFileDialog dlg = new NewLnkFileDialog())
                {
                    dlg.FileFilter = $"{AppString.Dialog.Program}|*.exe;*.bat;*.cmd;*.vbs;*.vbe;*.js;*.jse;*.wsf";
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    string lnkPath = $@"{SendToPath}\{ObjectPath.RemoveIllegalChars(dlg.ItemText)}.lnk";
                    lnkPath = ObjectPath.GetNewPathWithIndex(lnkPath, ObjectPath.PathType.File);
                    using(ShellLink shellLink = new ShellLink(lnkPath))
                    {
                        shellLink.TargetPath = dlg.ItemFilePath;
                        shellLink.WorkingDirectory = Path.GetDirectoryName(dlg.ItemFilePath);
                        shellLink.Arguments = dlg.Arguments;
                        shellLink.Save();
                    }
                    DesktopIni.SetLocalizedFileNames(lnkPath, dlg.ItemText);
                    this.InsertItem(new SendToItem(lnkPath), 2);
                }
            };
        }

        private void AddDirItem()
        {
            MyListItem item = new MyListItem
            {
                Text = Path.GetFileNameWithoutExtension(SendToPath),
                Image = ResourceIcon.GetFolderIcon(SendToPath).ToBitmap()
            };
            PictureButton btnPath = new PictureButton(AppImage.Open);
            ToolTipBox.SetToolTip(btnPath, AppString.Menu.FileLocation);
            btnPath.MouseDown += (sender, e) => ExternalProgram.OpenDirectory(SendToPath);
            item.AddCtr(btnPath);
            this.InsertItem(item, 1);
            item.ContextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem tsiRestoreDefault = new ToolStripMenuItem(AppString.Menu.RestoreDefault);
            item.ContextMenuStrip.Items.Add(tsiRestoreDefault);
            tsiRestoreDefault.Enabled = Directory.Exists(DefaultSendToPath);
            tsiRestoreDefault.Click += (sender, e) =>
            {
                if(AppMessageBox.Show(AppString.Message.RestoreDefault, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    File.SetAttributes(SendToPath, FileAttributes.Normal);
                    Directory.Delete(SendToPath, true);
                    Directory.CreateDirectory(SendToPath);
                    File.SetAttributes(SendToPath, File.GetAttributes(DefaultSendToPath));
                    foreach(string srcPath in Directory.GetFiles(DefaultSendToPath))
                    {
                        string dstPath = $@"{SendToPath}\{Path.GetFileName(srcPath)}";
                        File.Copy(srcPath, dstPath);
                    }
                    this.ClearItems();
                    this.LoadItems();
                }
            };
        }
    }
}