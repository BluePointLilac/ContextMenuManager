using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class SendToList : MyList
    {
        public static string SendToPath => Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\SendTo");
        private static string DesktopIniPath => $@"{SendToPath}\desktop.ini";

        public static IniReader DesktopIniReader;

        public static string GetMenuName(string fileName)
        {
            string name = DesktopIniReader.GetValue("LocalizedFileNames", fileName);
            return ResourceString.GetDirectString(name);
        }

        public void LoadItems()
        {
            this.ClearItems();
            this.LoadCommonItems();
            this.SortItemByText();
            this.AddNewItem();
            this.AddItem(new RegRuleItem(RegRuleItem.SendToDrive) { MarginRight = RegRuleItem.SysMarginRignt });
            this.AddItem(new RegRuleItem(RegRuleItem.DeferBuildSendTo) { MarginRight = RegRuleItem.SysMarginRignt });
        }

        private void LoadCommonItems()
        {
            DesktopIniReader = new IniReader(DesktopIniPath);
            Array.ForEach(new DirectoryInfo(SendToPath).GetFiles(), fi =>
            {
                if(fi.Name.ToLower() != "desktop.ini")
                    this.AddItem(new SendToItem(fi.FullName));
            });
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem();
            this.InsertItem(newItem, 0);
            newItem.NewItemAdd += (sender, e) =>
            {
                using(NewSendToDialog dlg = new NewSendToDialog())
                {
                    if(dlg.ShowDialog() == DialogResult.OK)
                        this.InsertItem(new SendToItem(dlg.FilePath), 2);
                }
            };
        }
    }
}