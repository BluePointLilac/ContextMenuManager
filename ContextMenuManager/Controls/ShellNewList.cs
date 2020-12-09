using BulePointLilac.Controls;
using BulePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellNewList : MyList
    {
        public const string ShellNewPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\ShellNew";
        private static readonly string[] ValueNames = { "NullFile", "Data", "FileName", "Directory", "Command" };

        public void LoadItems()
        {
            this.AddNewItem();
            this.AddItem(new LockNewItem(this));
            if(LockNewItem.IsLocked()) this.LoadLockItems();
            else this.LoadUnlockItems();
        }

        /// <summary>直接扫描所有扩展名</summary>
        private void LoadUnlockItems()
        {
            List<string> extensions = new List<string> { "Folder" };//文件夹
            using(RegistryKey root = Registry.ClassesRoot)
            {
                extensions.AddRange(Array.FindAll(root.GetSubKeyNames(), keyName => keyName.StartsWith(".")));
                if(WindowsOsVersion.IsBefore10) extensions.Add("Briefcase");//公文包(Win10没有)
                this.LoadItems(extensions);
            }
        }

        /// <summary>根据ShellNewPath的Classes键值扫描</summary>
        private void LoadLockItems()
        {
            string[] extensions = (string[])Registry.GetValue(ShellNewPath, "Classes", null);
            this.LoadItems(extensions.ToList());
        }

        private void LoadItems(List<string> extensions)
        {
            foreach(string extension in ShellNewItem.UnableSortExtensions)
            {
                string str = extensions.Find(ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
                if(str != null)
                {
                    extensions.Remove(str);
                    extensions.Insert(0, str);
                }
            }
            foreach(string extension in extensions)
            {
                using(RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(extension))
                {
                    string typeName = extKey.GetValue("")?.ToString();
                    if(typeName == null) continue;
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
                                    ShellNewItem item = new ShellNewItem(this, snKey.Name);
                                    if(item.ItemText != null) { this.AddItem(item); break; }
                                    else item.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void MoveItem(ShellNewItem item, bool isUp)
        {
            int index = this.GetItemIndex(item);
            int firstIndex = 0;
            for(int i = 0; i < this.Controls.Count; i++)
            {
                Control ctr = this.Controls[i];
                if(ctr.GetType() == typeof(ShellNewItem) && ((ShellNewItem)ctr).CanSort)
                {
                    firstIndex = i; break;
                }
            }
            if(isUp)
            {
                if(index > firstIndex)
                {
                    this.SetItemIndex(item, index - 1);
                }
            }
            else
            {
                if(index < this.Controls.Count - 1)
                {
                    this.SetItemIndex(item, index + 1);
                }
            }
            this.WriteRegistry();
        }

        public void WriteRegistry()
        {
            List<string> extensions = new List<string>();
            for(int i = 2; i < this.Controls.Count; i++)
            {
                extensions.Add(((ShellNewItem)Controls[i]).Extension);
            }
            LockNewItem.UnLock();
            Registry.SetValue(ShellNewPath, "Classes", extensions.ToArray());
            LockNewItem.Lock();
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem();
            this.AddItem(newItem);
            newItem.AddNewItem += (sender, e) =>
            {
                using(FileExtensionDialog dlg = new FileExtensionDialog())
                {
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    string extension = dlg.Extension;
                    foreach(Control ctr in this.Controls)
                    {
                        if(ctr.GetType() == typeof(ShellNewItem))
                        {
                            if(((ShellNewItem)ctr).Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBoxEx.Show(AppString.MessageBox.HasBeenAdded);
                                return;
                            }
                        }
                    }
                    string typeName = FileExtensionDialog.GetTypeName(extension, false);
                    using(RegistryKey exKey = Registry.ClassesRoot.OpenSubKey(extension, true))
                    {
                        exKey.SetValue("", typeName);
                        using(RegistryKey snKey = exKey.CreateSubKey("ShellNew", true))
                        {
                            snKey.SetValue("NullFile", string.Empty);
                            this.AddItem(new ShellNewItem(this, snKey.Name));
                            if(LockNewItem.IsLocked()) this.WriteRegistry();
                        }
                    }
                }
            };
        }
    }
}