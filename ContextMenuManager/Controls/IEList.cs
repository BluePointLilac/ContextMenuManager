using BluePointLilac.Controls;
using BluePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class IEList : MyList
    {
        public const string IEPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer";

        public void LoadItems()
        {
            AddNewItem();
            LoadIEItems();
        }

        private void LoadIEItems()
        {
            List<string> names = new List<string>();
            using(RegistryKey ieKey = RegistryEx.GetRegistryKey(IEPath))
            {
                if(ieKey == null) return;
                foreach(string part in IEItem.MeParts)
                {
                    using(RegistryKey meKey = ieKey.OpenSubKey(part))
                    {
                        if(meKey == null) continue;
                        foreach(string keyName in meKey.GetSubKeyNames())
                        {
                            if(names.Contains(keyName, StringComparer.OrdinalIgnoreCase)) continue;
                            using(RegistryKey key = meKey.OpenSubKey(keyName))
                            {
                                if(!string.IsNullOrEmpty(key.GetValue("")?.ToString()))
                                {
                                    this.AddItem(new IEItem(key.Name));
                                    names.Add(keyName);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem();
            this.AddItem(newItem);
            newItem.AddNewItem += () =>
            {
                using(NewIEDialog dlg = new NewIEDialog())
                {
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    this.InsertItem(new IEItem(dlg.RegPath), 1);
                }
            };
        }
    }
}