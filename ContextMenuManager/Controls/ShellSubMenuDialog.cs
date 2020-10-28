using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Microsoft.Win32.Registry;

namespace ContextMenuManager.Controls
{
    sealed class ShellSubMenuDialog : CommonDialog
    {
        public Icon Icon { get; set; }
        public string Text { get; set; }
        public override void Reset() { }
        protected override bool RunDialog(IntPtr hwndOwner) { return false; }

        /// <param name="parentPath">子菜单的父菜单的注册表路径</param>
        public void ShowDialog(string parentPath)
        {
            using(ShellSubMenuForm frm = new ShellSubMenuForm(parentPath))
            {
                frm.Text = this.Text;
                frm.Icon = this.Icon;
                frm.ShowDialog();
            }
        }

        sealed class ShellSubMenuForm : Form
        {
            /// <param name="parentPath">子菜单的父菜单的注册表路径</param>
            public ShellSubMenuForm(string parentPath)
            {
                this.ShowInTaskbar = false;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimumSize = this.Size = new Size(646, 389).DpiZoom();
                LstSubItems = new MyListBox { Dock = DockStyle.Fill, Parent = this };
                string value = GetValue(parentPath, "SubCommands", null)?.ToString();
                if(string.IsNullOrWhiteSpace(value))
                {
                    using(var shellKey = RegistryEx.GetRegistryKey($@"{parentPath}\shell"))
                    {
                        if(shellKey != null && shellKey.GetSubKeyNames().Length > 0)
                        {
                            new MultiItemsList(LstSubItems).LoadItems(parentPath);
                            return;
                        }
                        else
                        {
                            using(SubMenuModeForm frm = new SubMenuModeForm())
                            {
                                frm.ShowDialog();
                                if(frm.SubMenuMode == 1)
                                {
                                    new MultiItemsList(LstSubItems).LoadItems(parentPath);
                                    return;
                                }
                            }
                        }
                    }
                }
                new CommonMultiItemsList(LstSubItems).LoadItems(parentPath);
            }

            readonly MyListBox LstSubItems;

            sealed class CommonMultiItemsList : MyList
            {
                readonly List<string> SubKeyNames = new List<string>();
                /// <summary>子菜单的父菜单的注册表路径</summary>
                private string ParentPath { get; set; }
                /// <summary>菜单所处环境注册表路径</summary>
                private string ScenePath => RegistryEx.GetParentPath(RegistryEx.GetParentPath(ParentPath));

                readonly NewItem newItem = new NewItem();
                readonly AddCommonButton btnAddCommon = new AddCommonButton();
                readonly PictureButton btnAddExisting = new PictureButton(AppImage.AddExisting);
                readonly PictureButton btnAddSeparator = new PictureButton(AppImage.AddSeparator);

                public CommonMultiItemsList(MyListBox owner) : base(owner)
                {
                    this.AddItem(newItem);
                    newItem.AddCtrs(new[] { btnAddCommon, btnAddExisting, btnAddSeparator });
                    MyToolTip.SetToolTip(btnAddExisting, AppString.Tip.AddExistingItems);
                    MyToolTip.SetToolTip(btnAddSeparator, AppString.Tip.AddSeparator);
                    newItem.NewItemAdd += (sender, e) => AddNewItem();
                    btnAddCommon.MouseDown += (sender, e) => AddCommonItems();
                    btnAddExisting.MouseDown += (sender, e) => AddExistingItems();
                    btnAddSeparator.MouseDown += (sender, e) => AddSeparator();
                }

                /// <param name="parentPath">子菜单的父菜单的注册表路径</param>
                public void LoadItems(string parentPath)
                {
                    this.ParentPath = parentPath;
                    string value = GetValue(ParentPath, "SubCommands", null)?.ToString();
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
                    if(!CanAddMore()) return;
                    using(NewShellDialog dlg = new NewShellDialog
                    {
                        ScenePath = this.ScenePath,
                        ShellPath = ShellItem.CommandStorePath
                    })
                    {
                        if(dlg.ShowDialog() != DialogResult.OK) return;
                        SubKeyNames.Add(dlg.NewItemKeyName);
                        WriteRegistry();
                        SubShellItem item = new SubShellItem(this, dlg.NewItemKeyName);
                        this.AddItem(item);
                        this.HoveredItem = item;
                    }
                }

                private void AddCommonItems()
                {
                    if(!CanAddMore()) return;
                    using(ShellCommonDialog dlg = new ShellCommonDialog
                    {
                        ScenePath = this.ScenePath,
                        ShellPath = ShellItem.CommandStorePath
                    })
                    {
                        if(dlg.ShowDialog() == DialogResult.OK)
                        {
                            dlg.SelectedShellPaths.ForEach(path =>
                            {
                                string keyName = RegistryEx.GetKeyName(path);
                                this.AddItem(new SubShellItem(this, keyName));
                                SubKeyNames.Add(keyName);
                            });
                            WriteRegistry();
                        }
                    }
                }

                private void AddExistingItems()
                {
                    if(!CanAddMore()) return;
                    using(ShellStoreDialog dlg = new ShellStoreDialog())
                    {
                        if(dlg.ShowDialog() != DialogResult.OK) return;
                        dlg.SelectedKeyNames.ForEach(keyName =>
                        {
                            SubShellItem item = new SubShellItem(this, keyName);
                            this.AddItem(item);
                            this.SubKeyNames.Add(keyName);
                            WriteRegistry();
                        });
                        this.HoveredItem = (MyListItem)Controls[Controls.Count - 1];
                    }
                }

                private void AddSeparator()
                {
                    this.SubKeyNames.Add("|");
                    WriteRegistry();
                    SeparatorItem item = new SeparatorItem(this);
                    this.AddItem(item);
                    this.HoveredItem = item;
                }

                private bool CanAddMore()
                {
                    int count = 0;
                    foreach(Control item in Controls)
                    {
                        if(item.GetType() == typeof(SubShellItem)) count++;
                    }
                    bool flag = count < 16;
                    if(!flag) MessageBoxEx.Show(AppString.MessageBox.CannotAddNewItem);
                    return flag;
                }

                private void WriteRegistry()
                {
                    SetValue(ParentPath, "SubCommands", string.Join(";", SubKeyNames.ToArray()));
                }

                private static void MoveItem(MyListItem item, CommonMultiItemsList list, bool isUp)
                {
                    int index = list.GetItemIndex(item);
                    if(isUp)
                    {
                        if(index > 1)
                        {
                            list.SetItemIndex(item, index - 1);
                            list.SubKeyNames.Reverse(index - 2, 2);
                        }
                    }
                    else
                    {
                        if(index < list.Controls.Count - 1)
                        {
                            list.SetItemIndex(item, index + 1);
                            list.SubKeyNames.Reverse(index - 1, 2);
                        }
                    }
                    list.WriteRegistry();
                }

                private static void RemoveItem(CommonMultiItemsList list, MyListItem item)
                {
                    int index = list.GetItemIndex(item);
                    list.Controls.Remove(item);
                    list.Controls[index - 1].Focus();
                    list.SubKeyNames.RemoveAt(index - 1);
                    list.WriteRegistry();
                    item.Dispose();
                }

                sealed class SubShellItem : ShellItem, IBtnMoveUpDownItem
                {
                    public SubShellItem(CommonMultiItemsList list, string keyName) : base($@"{CommandStorePath}\{keyName}")
                    {
                        this.Owner = list;
                        BtnMoveDown = new MoveButton(this, false);
                        BtnMoveUp = new MoveButton(this, true);
                        BtnMoveUp.MouseDown += (sender, e) => MoveItem(this, Owner, true);
                        BtnMoveDown.MouseDown += (sender, e) => MoveItem(this, Owner, false);
                        ContextMenuStrip.Items.Remove(TsiDeleteMe);
                        ContextMenuStrip.Items.Add(TsiDeleteRef);
                        TsiDeleteRef.Click += (sender, e) => DeleteReference();
                    }

                    protected override bool IsSubItem => true;

                    readonly ToolStripMenuItem TsiDeleteRef = new ToolStripMenuItem(AppString.Menu.DeleteReference);
                    public CommonMultiItemsList Owner { get; private set; }
                    public MoveButton BtnMoveUp { get; set; }
                    public MoveButton BtnMoveDown { get; set; }

                    private void DeleteReference()
                    {
                        if(MessageBoxEx.Show(AppString.MessageBox.ConfirmDeleteReference,
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            RemoveItem(Owner, this);
                        }
                    }

                    public override void DeleteMe()
                    {
                        if(MessageBoxEx.Show(AppString.MessageBox.ConfirmDeleteReferenced,
                            MessageBoxButtons.YesNo) == DialogResult.Yes) base.DeleteMe();
                    }
                }

                sealed class SeparatorItem : MyListItem, IBtnDeleteItem, IBtnMoveUpDownItem
                {
                    public SeparatorItem(CommonMultiItemsList list)
                    {
                        this.Owner = list;
                        this.Text = AppString.Item.Separator;
                        this.Image = AppImage.Separator;
                        BtnDelete = new DeleteButton(this);
                        BtnMoveDown = new MoveButton(this, false);
                        BtnMoveUp = new MoveButton(this, true);
                        BtnMoveUp.MouseDown += (sender, e) => MoveItem(this, Owner, true);
                        BtnMoveDown.MouseDown += (sender, e) => MoveItem(this, Owner, false);
                        MyToolTip.SetToolTip(BtnDelete, AppString.Tip.Separator);
                    }

                    public DeleteButton BtnDelete { get; set; }

                    public CommonMultiItemsList Owner { get; private set; }
                    public MoveButton BtnMoveUp { get; set; }
                    public MoveButton BtnMoveDown { get; set; }

                    public void DeleteMe()
                    {
                        RemoveItem(Owner, this);
                        this.Dispose();
                    }
                }

                sealed class InvalidItem : MyListItem, IBtnDeleteItem, IBtnMoveUpDownItem
                {
                    public InvalidItem(CommonMultiItemsList list, string keyName)
                    {
                        this.Owner = list;
                        this.Text = $"{AppString.Item.InvalidItem} {keyName}";
                        this.Image = AppImage.NotFound.ToTransparent();
                        BtnDelete = new DeleteButton(this);
                        BtnMoveDown = new MoveButton(this, false);
                        BtnMoveUp = new MoveButton(this, true);
                        BtnMoveUp.MouseDown += (sender, e) => MoveItem(this, Owner, true);
                        BtnMoveDown.MouseDown += (sender, e) => MoveItem(this, Owner, false);
                        MyToolTip.SetToolTip(BtnDelete, AppString.Tip.InvalidItem);
                    }

                    public DeleteButton BtnDelete { get; set; }
                    public CommonMultiItemsList Owner { get; private set; }
                    public MoveButton BtnMoveUp { get; set; }
                    public MoveButton BtnMoveDown { get; set; }

                    public void DeleteMe()
                    {
                        RemoveItem(Owner, this);
                        this.Dispose();
                    }
                }
            }

            sealed class MultiItemsList : MyList
            {
                public MultiItemsList(MyListBox owner) : base(owner)
                {
                    this.AddItem(newItem);
                    newItem.AddCtr(btnAddCommon);
                    newItem.NewItemAdd += (sender, e) => AddNewItem();
                    btnAddCommon.MouseDown += (sender, e) => AddCommonItems();
                }

                readonly NewItem newItem = new NewItem();
                readonly AddCommonButton btnAddCommon = new AddCommonButton();
                /// <summary>子菜单的父菜单的注册表路径</summary>
                public string ParentPath { get; set; }
                /// <summary>子菜单的Shell项注册表路径</summary>
                private string ShellPath => $@"{ParentPath}\shell";
                /// <summary>菜单所处环境注册表路径</summary>
                private string ScenePath => RegistryEx.GetParentPath(RegistryEx.GetParentPath(ParentPath));


                public void LoadItems(string parentPath)
                {
                    this.ParentPath = parentPath;
                    using(var shellKey = RegistryEx.GetRegistryKey(ShellPath))
                    {
                        if(shellKey == null) return;
                        RegTrustedInstaller.TakeRegTreeOwnerShip(shellKey.Name);
                        Array.ForEach(shellKey.GetSubKeyNames(), keyName =>
                        {
                            this.AddItem(new SubShellItem($@"{ShellPath}\{keyName}"));
                        });
                    }
                }

                private void AddNewItem()
                {
                    if(!CanAddMore()) return;
                    using(NewShellDialog dlg = new NewShellDialog
                    {
                        ScenePath = this.ScenePath,
                        ShellPath = this.ShellPath
                    })
                    {
                        if(dlg.ShowDialog() == DialogResult.OK)
                            this.InsertItem(new SubShellItem(dlg.NewItemRegPath), GetItemIndex(newItem) + 1);
                    }
                }

                private void AddCommonItems()
                {
                    if(!CanAddMore()) return;
                    using(ShellCommonDialog dlg = new ShellCommonDialog
                    {
                        ScenePath = this.ScenePath,
                        ShellPath = this.ShellPath
                    })
                    {
                        if(dlg.ShowDialog() == DialogResult.OK)
                        {
                            dlg.SelectedShellPaths.ForEach(path => this.AddItem(new SubShellItem(path)));
                            this.SortItemByText();
                            this.SetItemIndex(newItem, 0);
                        }
                    }
                }

                private bool CanAddMore()
                {
                    int count = 0;
                    foreach(Control item in Controls)
                    {
                        if(item.GetType() == typeof(SubShellItem)) count++;
                    }
                    bool flag = count < 16;
                    if(!flag) MessageBoxEx.Show(AppString.MessageBox.CannotAddNewItem);
                    return flag;
                }

                sealed class SubShellItem : ShellItem
                {
                    public SubShellItem(string regPath) : base(regPath)
                    {
                        TsiOtherAttributes.DropDownItems.Add(tsiShowSeparator);
                        tsiShowSeparator.Click += (sender, e) => ShowSeparator = !tsiShowSeparator.Checked;
                        ContextMenuStrip.Opening += (sender, e) => tsiShowSeparator.Checked = ShowSeparator;
                    }

                    protected override bool IsSubItem => true;

                    private bool ShowSeparator
                    {
                        get
                        {
                            int value = Convert.ToInt32(GetValue(RegPath, "CommandFlags", 0)) % 64;
                            return value >= 32 && value < 56;
                        }
                        set
                        {
                            if(value) SetValue(RegPath, "CommandFlags", 32, Microsoft.Win32.RegistryValueKind.DWord);
                            else RegistryEx.DeleteValue(RegPath, "CommandFlags");
                        }
                    }

                    readonly ToolStripMenuItem tsiShowSeparator = new ToolStripMenuItem(AppString.Menu.ShowSeparator);
                }
            }
        }
    }
}