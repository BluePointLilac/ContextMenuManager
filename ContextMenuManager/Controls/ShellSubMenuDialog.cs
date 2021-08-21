using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using ContextMenuManager.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellSubMenuDialog : CommonDialog
    {
        public Icon Icon { get; set; }
        public string Text { get; set; }
        /// <summary>子菜单的父菜单的注册表路径</summary>
        public string ParentPath { get; set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            bool isPublic = true;
            string value = Microsoft.Win32.Registry.GetValue(this.ParentPath, "SubCommands", null)?.ToString();
            if(value == null) isPublic = false;
            else if(value.IsNullOrWhiteSpace())
            {
                using(var shellKey = RegistryEx.GetRegistryKey($@"{ParentPath}\shell"))
                {
                    if(shellKey != null && shellKey.GetSubKeyNames().Length > 0) isPublic = false;
                    else
                    {
                        string[] modes = new[] { ResourceString.Cancel, AppString.Dialog.Private, AppString.Dialog.Public };
                        string mode = MessageBoxEx.Show(AppString.Message.SelectSubMenuMode, AppString.General.AppName,
                            modes, MessageBoxImage.Question, null, modes[1]);
                        if(mode == modes[2]) isPublic = true;
                        else if(mode == modes[1]) isPublic = false;
                        else return false;
                    }
                }
            }

            using(SubItemsForm frm = new SubItemsForm())
            {
                frm.Text = this.Text;
                frm.Icon = this.Icon;
                frm.TopMost = AppConfig.TopMost;

                if(isPublic)
                {
                    frm.Text += $"({AppString.Dialog.Public})";
                    PulicMultiItemsList list = new PulicMultiItemsList();
                    frm.AddList(list);
                    list.ParentPath = this.ParentPath;
                    list.LoadItems();
                }
                else
                {
                    frm.Text += $"({AppString.Dialog.Private})";
                    PrivateMultiItemsList list = new PrivateMultiItemsList();
                    frm.AddList(list);
                    list.ParentPath = this.ParentPath;
                    list.LoadItems();
                }

                frm.ShowDialog();
            }
            return false;
        }

        sealed class PulicMultiItemsList : MyList
        {
            readonly List<string> SubKeyNames = new List<string>();
            /// <summary>子菜单的父菜单的注册表路径</summary>
            public string ParentPath { get; set; }
            /// <summary>菜单所处环境注册表路径</summary>
            private string ScenePath => RegistryEx.GetParentPath(RegistryEx.GetParentPath(ParentPath));

            readonly SubNewItem subNewItem = new SubNewItem(true);

            /// <param name="parentPath">子菜单的父菜单的注册表路径</param>
            public void LoadItems()
            {
                this.AddItem(subNewItem);
                subNewItem.AddNewItem += () => AddNewItem();
                subNewItem.AddExisting += () => AddReference();
                subNewItem.AddSeparator += () => AddSeparator();

                string value = Microsoft.Win32.Registry.GetValue(ParentPath, "SubCommands", null)?.ToString();
                Array.ForEach(value.Split(';'), cmd => SubKeyNames.Add(cmd.TrimStart()));
                SubKeyNames.RemoveAll(string.IsNullOrEmpty);

                using(var shellKey = RegistryEx.GetRegistryKey(ShellItem.CommandStorePath, false, true))
                {
                    foreach(string keyName in SubKeyNames)
                    {
                        using(var key = shellKey.OpenSubKey(keyName))
                        {
                            MyListItem item;
                            if(key != null) item = new SubShellItem(this, keyName);
                            else if(keyName == "|") item = new SeparatorItem(this);
                            else item = new InvalidItem(this, keyName);
                            this.AddItem(item);
                        }
                    }
                }
            }

            private void AddNewItem()
            {
                if(!SubShellTypeItem.CanAddMore(this)) return;
                using(NewShellDialog dlg = new NewShellDialog())
                {
                    dlg.ScenePath = this.ScenePath;
                    dlg.ShellPath = ShellItem.CommandStorePath;
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    SubKeyNames.Add(dlg.NewItemKeyName);
                    this.SaveSorting();
                    this.AddItem(new SubShellItem(this, dlg.NewItemKeyName));
                }
            }

            private void AddReference()
            {
                if(!SubShellTypeItem.CanAddMore(this)) return;
                using(ShellStoreDialog dlg = new ShellStoreDialog())
                {
                    dlg.IsReference = true;
                    dlg.ShellPath = ShellItem.CommandStorePath;
                    dlg.Filter = new Func<string, bool>(itemName => !(AppConfig.HideSysStoreItems
                        && itemName.StartsWith("Windows.", StringComparison.OrdinalIgnoreCase)));
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    foreach(string keyName in dlg.SelectedKeyNames)
                    {
                        if(!SubShellTypeItem.CanAddMore(this)) return;
                        this.AddItem(new SubShellItem(this, keyName));
                        this.SubKeyNames.Add(keyName);
                        this.SaveSorting();
                    }
                }
            }

            private void AddSeparator()
            {
                if(this.Controls[this.Controls.Count - 1] is SeparatorItem) return;
                this.SubKeyNames.Add("|");
                this.SaveSorting();
                this.AddItem(new SeparatorItem(this));
            }

            private void SaveSorting()
            {
                Microsoft.Win32.Registry.SetValue(ParentPath, "SubCommands", string.Join(";", SubKeyNames.ToArray()));
            }

            private void MoveItem(MyListItem item, bool isUp)
            {
                int index = this.GetItemIndex(item);
                if(isUp)
                {
                    if(index > 1)
                    {
                        this.SetItemIndex(item, index - 1);
                        this.SubKeyNames.Reverse(index - 2, 2);
                    }
                }
                else
                {
                    if(index < this.Controls.Count - 1)
                    {
                        this.SetItemIndex(item, index + 1);
                        this.SubKeyNames.Reverse(index - 1, 2);
                    }
                }
                this.SaveSorting();
            }

            private void DeleteItem(MyListItem item)
            {
                int index = this.GetItemIndex(item);
                this.SubKeyNames.RemoveAt(index - 1);
                if(index == this.Controls.Count - 1) index--;
                this.Controls.Remove(item);
                this.Controls[index].Focus();
                this.SaveSorting();
                item.Dispose();
            }

            sealed class SubShellItem : SubShellTypeItem
            {
                public SubShellItem(PulicMultiItemsList list, string keyName) : base($@"{CommandStorePath}\{keyName}")
                {
                    this.Owner = list;
                    BtnMoveUp.MouseDown += (sender, e) => Owner.MoveItem(this, true);
                    BtnMoveDown.MouseDown += (sender, e) => Owner.MoveItem(this, false);
                    ContextMenuStrip.Items.Remove(TsiDeleteMe);
                    ContextMenuStrip.Items.Add(TsiDeleteRef);
                    TsiDeleteRef.Click += (sender, e) => DeleteReference();
                }

                readonly ToolStripMenuItem TsiDeleteRef = new ToolStripMenuItem(AppString.Menu.DeleteReference);
                public PulicMultiItemsList Owner { get; private set; }

                private void DeleteReference()
                {
                    if(AppMessageBox.Show(AppString.Message.ConfirmDeleteReference, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Owner.DeleteItem(this);
                    }
                }
            }

            sealed class SeparatorItem : SubSeparatorItem
            {
                public SeparatorItem(PulicMultiItemsList list) : base()
                {
                    this.Owner = list;
                    BtnMoveUp.MouseDown += (sender, e) => Owner.MoveItem(this, true);
                    BtnMoveDown.MouseDown += (sender, e) => Owner.MoveItem(this, false);
                }

                public PulicMultiItemsList Owner { get; private set; }

                public override void DeleteMe()
                {
                    Owner.DeleteItem(this);
                }
            }

            sealed class InvalidItem : MyListItem, IBtnDeleteItem, IBtnMoveUpDownItem
            {
                public InvalidItem(PulicMultiItemsList list, string keyName)
                {
                    this.Owner = list;
                    this.Text = $"{AppString.Other.InvalidItem} {keyName}";
                    this.Image = AppImage.NotFound.ToTransparent();
                    BtnDelete = new DeleteButton(this);
                    BtnMoveDown = new MoveButton(this, false);
                    BtnMoveUp = new MoveButton(this, true);
                    BtnMoveUp.MouseDown += (sender, e) => Owner.MoveItem(this, true);
                    BtnMoveDown.MouseDown += (sender, e) => Owner.MoveItem(this, false);
                    ToolTipBox.SetToolTip(this, AppString.Tip.InvalidItem);
                    ToolTipBox.SetToolTip(BtnDelete, AppString.Menu.Delete);
                }

                public DeleteButton BtnDelete { get; set; }
                public PulicMultiItemsList Owner { get; private set; }
                public MoveButton BtnMoveUp { get; set; }
                public MoveButton BtnMoveDown { get; set; }

                public void DeleteMe()
                {
                    Owner.DeleteItem(this);
                }
            }
        }

        sealed class PrivateMultiItemsList : MyList
        {
            readonly SubNewItem subNewItem = new SubNewItem(false);

            /// <summary>父菜单的注册表路径</summary>
            public string ParentPath { get; set; }
            /// <summary>子菜单的Shell项注册表路径</summary>
            private string ShellPath { get; set; }
            /// <summary>父菜单的Shell项注册表路径</summary>
            private string ParentShellPath => RegistryEx.GetParentPath(ParentPath);
            /// <summary>菜单所处环境注册表路径</summary>
            private string ScenePath => RegistryEx.GetParentPath(ParentShellPath);
            /// <summary>父菜单的项名</summary>
            private string ParentKeyName => RegistryEx.GetKeyName(ParentPath);

            public void LoadItems()
            {
                this.AddItem(subNewItem);
                subNewItem.AddNewItem += () => AddNewItem();
                subNewItem.AddSeparator += () => AddSeparator();
                subNewItem.AddExisting += () => AddFromParentMenu();

                string sckValue = Microsoft.Win32.Registry.GetValue(this.ParentPath, "ExtendedSubCommandsKey", null)?.ToString();
                if(!sckValue.IsNullOrWhiteSpace())
                {
                    this.ShellPath = $@"{RegistryEx.CLASSES_ROOT}\{sckValue}\shell";
                }
                else
                {
                    this.ShellPath = $@"{this.ParentPath}\shell";
                }
                using(var shellKey = RegistryEx.GetRegistryKey(ShellPath))
                {
                    if(shellKey == null) return;
                    RegTrustedInstaller.TakeRegTreeOwnerShip(shellKey.Name);
                    foreach(string keyName in shellKey.GetSubKeyNames())
                    {
                        string regPath = $@"{ShellPath}\{keyName}";
                        int value = Convert.ToInt32(Microsoft.Win32.Registry.GetValue(regPath, "CommandFlags", 0));
                        if(value % 16 >= 8)
                        {
                            this.AddItem(new SeparatorItem(this, regPath));
                        }
                        else
                        {
                            this.AddItem(new SubShellItem(this, regPath));
                        }
                    }
                }
            }

            private void AddNewItem()
            {
                if(!SubShellTypeItem.CanAddMore(this)) return;
                using(NewShellDialog dlg = new NewShellDialog
                {
                    ScenePath = this.ScenePath,
                    ShellPath = this.ShellPath
                })
                {
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    this.AddItem(new SubShellItem(this, dlg.NewItemRegPath));
                }
            }

            private void AddSeparator()
            {
                if(this.Controls[this.Controls.Count - 1] is SeparatorItem) return;
                string regPath;
                if(this.Controls.Count > 1)
                {
                    regPath = GetItemRegPath((MyListItem)Controls[Controls.Count - 1]);
                }
                else
                {
                    regPath = $@"{ShellPath}\Item";
                }
                regPath = ObjectPath.GetNewPathWithIndex(regPath, ObjectPath.PathType.Registry);
                Microsoft.Win32.Registry.SetValue(regPath, "CommandFlags", 0x8);
                this.AddItem(new SeparatorItem(this, regPath));
            }

            private void AddFromParentMenu()
            {
                if(!SubShellTypeItem.CanAddMore(this)) return;
                using(ShellStoreDialog dlg = new ShellStoreDialog())
                {
                    dlg.IsReference = false;
                    dlg.ShellPath = this.ParentShellPath;
                    dlg.Filter = new Func<string, bool>(itemName => !itemName.Equals(this.ParentKeyName, StringComparison.OrdinalIgnoreCase));
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    foreach(string keyName in dlg.SelectedKeyNames)
                    {
                        if(!SubShellTypeItem.CanAddMore(this)) return;
                        string srcPath = $@"{dlg.ShellPath}\{keyName}";
                        string dstPath = ObjectPath.GetNewPathWithIndex($@"{ShellPath}\{keyName}", ObjectPath.PathType.Registry);

                        RegistryEx.CopyTo(srcPath, dstPath);
                        this.AddItem(new SubShellItem(this, dstPath));
                    }
                }
            }

            public void MoveItem(MyListItem item, bool isUp)
            {
                int index = this.GetItemIndex(item);
                MyListItem otherItem = null;
                if(isUp)
                {
                    if(index > 1)
                    {
                        otherItem = (MyListItem)this.Controls[index - 1];
                        this.SetItemIndex(item, index - 1);
                    }
                }
                else
                {
                    if(index < this.Controls.Count - 1)
                    {
                        otherItem = (MyListItem)this.Controls[index + 1];
                        this.SetItemIndex(item, index + 1);
                    }
                }
                if(otherItem != null)
                {
                    string path1 = GetItemRegPath(item);
                    string path2 = GetItemRegPath(otherItem);
                    string tempPath = ObjectPath.GetNewPathWithIndex(path1, ObjectPath.PathType.Registry);
                    RegistryEx.MoveTo(path1, tempPath);
                    RegistryEx.MoveTo(path2, path1);
                    RegistryEx.MoveTo(tempPath, path2);
                    SetItemRegPath(item, path2);
                    SetItemRegPath(otherItem, path1);
                }
            }

            private string GetItemRegPath(MyListItem item)
            {
                PropertyInfo pi = item.GetType().GetProperty("RegPath");
                return pi.GetValue(item, null).ToString();
            }

            private void SetItemRegPath(MyListItem item, string regPath)
            {
                PropertyInfo pi = item.GetType().GetProperty("RegPath");
                pi.SetValue(item, regPath, null);
            }

            sealed class SubShellItem : SubShellTypeItem
            {
                public SubShellItem(PrivateMultiItemsList list, string regPath) : base(regPath)
                {
                    this.Owner = list;
                    BtnMoveUp.MouseDown += (sender, e) => Owner.MoveItem(this, true);
                    BtnMoveDown.MouseDown += (sender, e) => Owner.MoveItem(this, false);
                    SetItemTextValue();
                }

                public PrivateMultiItemsList Owner { get; private set; }

                private void SetItemTextValue()
                {
                    using(var key = RegistryEx.GetRegistryKey(this.RegPath, true))
                    {
                        bool hasValue = false;
                        foreach(string valueName in new[] { "MUIVerb", "" })
                        {
                            if(key.GetValue(valueName) != null)
                            {
                                hasValue = true; break;
                            }
                        }
                        if(!hasValue) key.SetValue("MUIVerb", this.ItemText);
                    }

                }
            }

            sealed class SeparatorItem : SubSeparatorItem
            {
                public SeparatorItem(PrivateMultiItemsList list, string regPath)
                {
                    this.Owner = list;
                    this.RegPath = regPath;
                    BtnMoveUp.MouseDown += (sender, e) => Owner.MoveItem(this, true);
                    BtnMoveDown.MouseDown += (sender, e) => Owner.MoveItem(this, false);
                }

                public PrivateMultiItemsList Owner { get; private set; }
                public string RegPath { get; private set; }

                public override void DeleteMe()
                {
                    RegistryEx.DeleteKeyTree(this.RegPath);
                    int index = this.Parent.Controls.GetChildIndex(this);
                    if(index == this.Parent.Controls.Count - 1) index--;
                    this.Parent.Controls[index].Focus();
                    this.Parent.Controls.Remove(this);
                    this.Dispose();
                }
            }
        }

        class SubSeparatorItem : MyListItem, IBtnDeleteItem, IBtnMoveUpDownItem
        {
            public SubSeparatorItem()
            {
                this.Text = AppString.Other.Separator;
                this.HasImage = false;
                BtnDelete = new DeleteButton(this);
                BtnMoveDown = new MoveButton(this, false);
                BtnMoveUp = new MoveButton(this, true);
                ToolTipBox.SetToolTip(BtnDelete, AppString.Menu.Delete);
            }

            public DeleteButton BtnDelete { get; set; }
            public MoveButton BtnMoveUp { get; set; }
            public MoveButton BtnMoveDown { get; set; }

            public virtual void DeleteMe() { }
        }

        class SubShellTypeItem : ShellItem, IBtnMoveUpDownItem
        {
            public SubShellTypeItem(string regPath) : base(regPath)
            {
                BtnMoveDown = new MoveButton(this, false);
                BtnMoveUp = new MoveButton(this, true);
                this.SetCtrIndex(BtnMoveDown, 1);
                this.SetCtrIndex(BtnMoveUp, 2);
            }

            public MoveButton BtnMoveUp { get; set; }
            public MoveButton BtnMoveDown { get; set; }

            protected override bool IsSubItem => true;

            public static bool CanAddMore(MyList list)
            {
                int count = 0;
                foreach(Control item in list.Controls)
                {
                    if(item.GetType().BaseType == typeof(SubShellTypeItem)) count++;
                }
                bool flag = count < 16;
                if(!flag) AppMessageBox.Show(AppString.Message.CannotAddNewItem);
                return flag;
            }
        }

        sealed class SubNewItem : NewItem
        {
            public SubNewItem(bool isPublic)
            {
                this.AddCtrs(new[] { btnAddExisting, btnAddSeparator });
                ToolTipBox.SetToolTip(btnAddExisting, isPublic ? AppString.Tip.AddReference : AppString.Tip.AddFromParentMenu);
                ToolTipBox.SetToolTip(btnAddSeparator, AppString.Tip.AddSeparator);
                btnAddExisting.MouseDown += (sender, e) => AddExisting?.Invoke();
                btnAddSeparator.MouseDown += (sender, e) => AddSeparator?.Invoke();
            }

            readonly PictureButton btnAddExisting = new PictureButton(AppImage.AddExisting);
            readonly PictureButton btnAddSeparator = new PictureButton(AppImage.AddSeparator);

            public Action AddExisting;
            public Action AddSeparator;
        }
    }
}