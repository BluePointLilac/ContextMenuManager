using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellNewList : MyList
    {
        public const string ShellNewPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\ShellNew";

        public ShellNewSeparator Separator;

        public void LoadItems()
        {
            this.AddNewItem();
            this.AddItem(new ShellNewLockItem(this));
            Separator = new ShellNewSeparator();
            this.AddItem(Separator);
            if(ShellNewLockItem.IsLocked) this.LoadLockItems();
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
                if(extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    extensions.Remove(extension);
                    extensions.Insert(0, extension);
                }
            }
            using(RegistryKey root = Registry.ClassesRoot)
            {
                foreach(string extension in extensions)
                {
                    using(RegistryKey extKey = root.OpenSubKey(extension))
                    {
                        string defalutOpenMode = extKey?.GetValue("")?.ToString();
                        if(string.IsNullOrEmpty(defalutOpenMode)) continue;
                        using(RegistryKey openModeKey = root.OpenSubKey(defalutOpenMode))
                        {
                            if(openModeKey == null) continue;
                            string value1 = openModeKey.GetValue("FriendlyTypeName")?.ToString();
                            string value2 = openModeKey.GetValue("")?.ToString();
                            value1 = ResourceString.GetDirectString(value1);
                            if(value1.IsNullOrWhiteSpace() && value2.IsNullOrWhiteSpace()) continue;
                        }
                        using(RegistryKey tKey = extKey.OpenSubKey(defalutOpenMode))
                        {
                            foreach(string part in ShellNewItem.SnParts)
                            {
                                string snPart = part;
                                if(tKey != null) snPart = $@"{defalutOpenMode}\{snPart}";
                                using(RegistryKey snKey = extKey.OpenSubKey(snPart))
                                {
                                    if(ShellNewItem.EffectValueNames.Any(valueName => snKey?.GetValue(valueName) != null))
                                    {
                                        ShellNewItem item = new ShellNewItem(this, snKey.Name);
                                        if(item.BeforeSeparator)
                                        {
                                            int index2 = this.GetItemIndex(Separator);
                                            this.InsertItem(item, index2);
                                        }
                                        else
                                        {
                                            this.AddItem(item);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void MoveItem(ShellNewItem shellNewItem, bool isUp)
        {
            int index = this.GetItemIndex(shellNewItem);
            index += isUp ? -1 : 1;
            if(index == this.Controls.Count) return;
            Control ctr = this.Controls[index];
            if(ctr is ShellNewItem item && item.CanSort)
            {
                this.SetItemIndex(shellNewItem, index);
                this.SaveSorting();
            }
        }

        public void SaveSorting()
        {
            List<string> extensions = new List<string>();
            for(int i = 2; i < this.Controls.Count; i++)
            {
                if(Controls[i] is ShellNewItem item)
                {
                    extensions.Add(item.Extension);
                }
            }
            ShellNewLockItem.UnLock();
            Registry.SetValue(ShellNewPath, "Classes", extensions.ToArray());
            ShellNewLockItem.Lock();
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
                    if(extension == ".") return;
                    string openMode = FileExtension.GetOpenMode(extension);
                    if(string.IsNullOrEmpty(openMode))
                    {
                        MessageBoxEx.Show(AppString.Message.NoOpenModeExtension);
                        ExternalProgram.ShowOpenWithDialog(extension);
                        return;
                    }
                    foreach(Control ctr in this.Controls)
                    {
                        if(ctr is ShellNewItem item)
                        {
                            if(item.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBoxEx.Show(AppString.Message.HasBeenAdded);
                                return;
                            }
                        }
                    }

                    using(RegistryKey root = Registry.ClassesRoot)
                    using(RegistryKey exKey = root.OpenSubKey(extension, true))
                    using(RegistryKey snKey = exKey.CreateSubKey("ShellNew", true))
                    {
                        string defaultOpenMode = exKey.GetValue("")?.ToString();
                        if(string.IsNullOrEmpty(defaultOpenMode))
                        {
                            exKey.SetValue("", openMode);
                        }

                        snKey.SetValue("NullFile", string.Empty);
                        ShellNewItem item = new ShellNewItem(this, snKey.Name);
                        this.AddItem(item);
                        item.Focus();
                        if(item.ItemText.IsNullOrWhiteSpace())
                        {
                            item.ItemText = FileExtension.GetFriendlyDocName(extension);
                        }
                        if(ShellNewLockItem.IsLocked) this.SaveSorting();
                    }
                }
            };
        }

        public sealed class ShellNewLockItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiWebSearchItem, ITsiRegPathItem
        {
            public ShellNewLockItem(ShellNewList list)
            {
                this.Owner = list;
                this.Image = AppImage.Lock;
                this.Text = AppString.Other.LockNewMenu;
                BtnShowMenu = new MenuButton(this);
                ChkVisible = new VisibleCheckBox(this) { Checked = IsLocked };
                MyToolTip.SetToolTip(ChkVisible, AppString.Tip.LockNewMenu);
                TsiSearch = new WebSearchMenuItem(this);
                TsiRegLocation = new RegLocationMenuItem(this);
                this.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
                    { TsiSearch, new ToolStripSeparator(), TsiRegLocation });
            }

            public MenuButton BtnShowMenu { get; set; }
            public WebSearchMenuItem TsiSearch { get; set; }
            public RegLocationMenuItem TsiRegLocation { get; set; }
            public VisibleCheckBox ChkVisible { get; set; }
            public ShellNewList Owner { get; private set; }

            public bool ItemVisible
            {
                get => IsLocked;
                set
                {
                    if(value) Owner.SaveSorting();
                    else UnLock();
                    foreach(Control ctr in Owner.Controls)
                    {
                        if(ctr is ShellNewItem item)
                        {
                            item.SetSortabled(value);
                        }
                    }
                }
            }

            public string SearchText => Text;
            public string RegPath => ShellNewPath;
            public string ValueName => "Classes";

            public static bool IsLocked
            {
                get
                {
                    using(RegistryKey key = RegistryEx.GetRegistryKey(ShellNewPath))
                    {
                        RegistrySecurity rs = key.GetAccessControl();
                        foreach(RegistryAccessRule rar in rs.GetAccessRules(true, true, typeof(NTAccount)))
                        {
                            if(rar.AccessControlType.ToString().Equals("Deny", StringComparison.OrdinalIgnoreCase))
                            {
                                if(rar.IdentityReference.ToString().Equals("Everyone", StringComparison.OrdinalIgnoreCase)) return true;
                            }
                        }
                    }
                    return false;
                }
            }

            public static void Lock()
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(ShellNewPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions))
                {
                    RegistrySecurity rs = new RegistrySecurity();
                    RegistryAccessRule rar = new RegistryAccessRule("Everyone", RegistryRights.Delete | RegistryRights.WriteKey, AccessControlType.Deny);
                    rs.AddAccessRule(rar);
                    key.SetAccessControl(rs);
                }
            }

            public static void UnLock()
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(ShellNewPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions))
                {
                    RegistrySecurity rs = key.GetAccessControl();
                    foreach(RegistryAccessRule rar in rs.GetAccessRules(true, true, typeof(NTAccount)))
                    {
                        if(rar.AccessControlType.ToString().Equals("Deny", StringComparison.OrdinalIgnoreCase))
                        {
                            if(rar.IdentityReference.ToString().Equals("Everyone", StringComparison.OrdinalIgnoreCase))
                            {
                                rs.RemoveAccessRule(rar);
                            }
                        }
                    }
                    key.SetAccessControl(rs);
                }
            }
        }

        public sealed class ShellNewSeparator : MyListItem
        {
            public ShellNewSeparator()
            {
                this.Text = AppString.Other.Separator;
                this.HasImage = false;
            }
        }
    }
}