using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class SendToList : MyList
    {
        public static readonly string SendToPath = Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\SendTo");
        public static readonly string DesktopIniPath = $@"{SendToPath}\desktop.ini";
        public static IniWriter DesktopIniWriter = new IniWriter(DesktopIniPath);
        public static IniReader DesktopIniReader;

        public void LoadItems()
        {
            DesktopIniReader = new IniReader(DesktopIniPath);
            Array.ForEach(new DirectoryInfo(SendToPath).GetFiles(), fi =>
            {
                if(fi.Name.ToLower() != "desktop.ini")
                    this.AddItem(new SendToItem(fi.FullName));
            });
            this.SortItemByText();
            this.AddNewItem();
            this.AddDirItem();
            this.AddItem(new RegRuleItem(RegRuleItem.SendToDrive));
            this.AddItem(new RegRuleItem(RegRuleItem.DeferBuildSendTo));
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem();
            this.InsertItem(newItem, 0);
            newItem.AddNewItem += (sender, e) =>
            {
                using(NewSendToDialog dlg = new NewSendToDialog())
                {
                    if(dlg.ShowDialog() == DialogResult.OK)
                        this.InsertItem(new SendToItem(dlg.FilePath), 2);
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
            btnPath.MouseDown += (sender, e) => Process.Start(SendToPath);
            item.AddCtr(btnPath);
            item.SetNoClickEvent();
            this.InsertItem(item, 1);
        }
    }
}