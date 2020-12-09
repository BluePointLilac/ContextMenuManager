using BulePointLilac.Controls;
using BulePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class OpenWithList : MyList
    {
        public void LoadItems()
        {
            this.LoadCommonItems();
            this.SortItemByText();
            this.AddNewItem();
            RegRuleItem storeItem = new RegRuleItem(RegRuleItem.UseStoreOpenWith)
            {
                MarginRight = RegRuleItem.SysMarginRignt,
                //Win8、Win8.1、Win10才有在应用商店中查找应用
                Visible = WindowsOsVersion.ISAfterOrEqual8
            };
            this.InsertItem(storeItem, 1);
        }

        private void LoadCommonItems()
        {
            using(RegistryKey appKey = Registry.ClassesRoot.OpenSubKey("Applications"))
            {
                foreach(string appName in appKey.GetSubKeyNames())
                {
                    if(!appName.Contains('.')) continue;//需要为有扩展名的文件名
                    using(RegistryKey shellKey = appKey.OpenSubKey($@"{appName}\shell"))
                    {
                        if(shellKey == null) continue;

                        List<string> names = shellKey.GetSubKeyNames().ToList();
                        if(names.Contains("open", StringComparer.OrdinalIgnoreCase)) names.Insert(0, "open");

                        string keyName = names.Find(name =>
                        {
                            using(var cmdKey = shellKey.OpenSubKey(name))
                                return cmdKey.GetValue("NeverDefault") == null;
                        });
                        if(keyName == null) continue;

                        using(RegistryKey commandKey = shellKey.OpenSubKey($@"{keyName}\command"))
                        {
                            string command = commandKey?.GetValue("")?.ToString();
                            if(ObjectPath.ExtractFilePath(command) != null)
                                this.AddItem(new OpenWithItem(commandKey.Name));
                        }
                    }
                }
            }
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem();
            this.InsertItem(newItem, 0);
            newItem.AddNewItem += (sender, e) =>
            {
                using(NewOpenWithDialog dlg = new NewOpenWithDialog())
                {
                    if(dlg.ShowDialog() == DialogResult.OK)
                        this.InsertItem(new OpenWithItem(dlg.RegPath), 2);
                }
            };
        }
    }
}