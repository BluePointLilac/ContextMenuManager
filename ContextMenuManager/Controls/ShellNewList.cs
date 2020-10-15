using BulePointLilac.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellNewList : MyList
    {
        private static readonly string[] ValueNames = { "NullFile", "Data", "FileName", "Directory" };

        public void LoadItems()
        {
            this.ClearItems();
            this.AddNewItem();
            this.LoadCommonItems();
        }

        private void LoadCommonItems()
        {
            List<string> extensions = new List<string> { "Folder" };
            using(RegistryKey root = Registry.ClassesRoot)
            {
                extensions.AddRange(Array.FindAll(root.GetSubKeyNames(), keyName => keyName.StartsWith(".")));
                foreach(string extension in extensions)
                {
                    string typeName = FileExtensionDialog.GetTypeName(extension, false);
                    if(typeName == null) continue;
                    using(RegistryKey extKey = root.OpenSubKey(extension))
                    using(RegistryKey tKey = extKey.OpenSubKey(typeName))
                    {
                        foreach(string part in ShellNewItem.SnParts)
                        {
                            string snPart = part;
                            if(tKey != null) snPart = $@"{typeName}\{snPart}";
                            using(RegistryKey snKey = extKey.OpenSubKey(snPart))
                            {
                                if(ValueNames.Any(valueName => snKey?.GetValue(valueName) != null))
                                {
                                    ShellNewItem item = new ShellNewItem(snKey.Name);
                                    if(item.ItemText != null) { this.AddItem(item); break; }
                                    else item.Dispose();
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
            newItem.NewItemAdd += (sender, e) =>
            {
                using(FileExtensionDialog dlg = new FileExtensionDialog())
                {
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    string extension = dlg.Extension;
                    string typeName = FileExtensionDialog.GetTypeName(extension, false);
                    using(RegistryKey exKey = Registry.ClassesRoot.OpenSubKey(extension, true))
                    {
                        exKey.SetValue("", typeName);
                        using(RegistryKey snKey = exKey.CreateSubKey("ShellNew", true))
                        {
                            snKey.SetValue("NullFile", string.Empty);
                            this.InsertItem(new ShellNewItem(snKey.Name), GetItemIndex(newItem) + 1);
                        }
                    }
                }
            };
        }
    }
}