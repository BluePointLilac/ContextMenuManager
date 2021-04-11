using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class SendToList : MyList
    {
        public static readonly string SendToPath = Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\SendTo");

        public void LoadItems()
        {
            Array.ForEach(Directory.GetFiles(SendToPath), path =>
            {
                if(Path.GetFileName(path).ToLower() != "desktop.ini")
                    this.AddItem(new SendToItem(path));
            });
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
            newItem.AddNewItem += (sender, e) =>
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
            MyToolTip.SetToolTip(btnPath, AppString.Menu.FileLocation);
            btnPath.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(SendToPath);
            item.AddCtr(btnPath);
            this.InsertItem(item, 1);
        }
    }
}