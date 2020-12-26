using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class LockNewItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiWebSearchItem
    {
        public LockNewItem(ShellNewList list)
        {
            this.Owner = list;
            this.Image = AppImage.Lock;
            this.Text = AppString.Item.LockNewMenu;
            this.SetNoClickEvent();
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this) { Checked = IsLocked() };
            MyToolTip.SetToolTip(ChkVisible, AppString.Tip.LockNewMenu);
            TsiSearch = new WebSearchMenuItem(this);
            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.Add(TsiSearch);
        }

        public MenuButton BtnShowMenu { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public ShellNewList Owner { get; private set; }

        public bool ItemVisible
        {
            get => IsLocked();
            set
            {
                if(value) Lock();
                else UnLock();
                foreach(Control ctr in Owner.Controls)
                {
                    if(ctr.GetType() == typeof(ShellNewItem))
                    {
                        ShellNewItem item = (ShellNewItem)ctr;
                        if(item.CanSort)
                        {
                            item.BtnMoveDown.Visible = item.BtnMoveUp.Visible = value;
                        }
                    }
                }
            }
        }

        public string SearchText => Text;


        public static bool IsLocked()
        {
            using(RegistryKey key = RegistryEx.GetRegistryKey(ShellNewList.ShellNewPath))
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

        public static void Lock()
        {
            using(RegistryKey key = RegistryEx.GetRegistryKey(ShellNewList.ShellNewPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions))
            {
                RegistrySecurity rs = new RegistrySecurity();
                RegistryAccessRule rar = new RegistryAccessRule("Everyone", RegistryRights.Delete | RegistryRights.WriteKey, AccessControlType.Deny);
                rs.AddAccessRule(rar);
                key.SetAccessControl(rs);
            }
        }

        public static void UnLock()
        {
            using(RegistryKey key = RegistryEx.GetRegistryKey(ShellNewList.ShellNewPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions))
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
}