using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
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
            using(ShellSubMenuForm frm = new ShellSubMenuForm())
            {
                frm.Text = this.Text;
                frm.Icon = this.Icon;
                frm.ParentPath = parentPath;
                frm.ShowDialog();
            }
        }

        sealed class ShellSubMenuForm : Form
        {
            public ShellSubMenuForm()
            {
                this.StartPosition = FormStartPosition.CenterParent;
                this.ShowInTaskbar = this.MaximizeBox = this.MinimizeBox = false;
                this.MinimumSize = this.Size = new Size(646, 369).DpiZoom();
                this.Controls.AddRange(new Control[] { MlbSubItems, StatusBar });
                StatusBar.CanMoveForm();
                this.OnResize(null);
            }

            /// <summary>子菜单的父菜单的注册表路径</summary>
            public string ParentPath { get; set; }
            readonly MyListBox MlbSubItems = new MyListBox { Dock = DockStyle.Fill };
            readonly MyStatusBar StatusBar = new MyStatusBar();
            private MyList LstSubItems;

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                bool isPublic = true;
                string value = GetValue(ParentPath, "SubCommands", null)?.ToString();
                if(value == null) isPublic = false;
                else if(value.IsNullOrWhiteSpace())
                {
                    using(var shellKey = RegistryEx.GetRegistryKey($@"{ParentPath}\shell"))
                    {
                        if(shellKey != null && shellKey.GetSubKeyNames().Length > 0) isPublic = false;
                        else
                        {
                            using(SubMenuModeForm frm = new SubMenuModeForm())
                            {
                                frm.ShowDialog();
                                switch(frm.Mode)
                                {
                                    case SubMenuModeForm.SubMode.Public:
                                        isPublic = true; break;
                                    case SubMenuModeForm.SubMode.Private:
                                        isPublic = false; break;
                                    case SubMenuModeForm.SubMode.None:
                                        this.Dispose(); return;
                                }
                            }
                        }
                    }
                }
                if(isPublic)
                {
                    LstSubItems = new PulicMultiItemsList(MlbSubItems);
                    ((PulicMultiItemsList)LstSubItems).LoadItems(ParentPath);
                    this.Text += $"({AppString.Dialog.Public})";
                }
                else
                {
                    LstSubItems = new PrivateMultiItemsList(MlbSubItems);
                    ((PrivateMultiItemsList)LstSubItems).LoadItems(ParentPath);
                    this.Text += $"({AppString.Dialog.Private})";
                }
                LstSubItems.HoveredItemChanged += (sender, a) =>
                {
                    if(!AppConfig.ShowFilePath) return;
                    MyListItem item = LstSubItems.HoveredItem;
                    if(item is ITsiFilePathItem pathItem)
                    {
                        string path = pathItem.ItemFilePath;
                        if(File.Exists(path)) { StatusBar.Text = path; return; }
                    }
                    StatusBar.Text = item.Text;
                };
            }

            sealed class SubMenuModeForm : Form
            {
                public SubMenuModeForm()
                {
                    this.Text = AppString.General.AppName;
                    this.ShowIcon = this.ShowInTaskbar = false;
                    this.MinimizeBox = this.MaximizeBox = false;
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                    this.StartPosition = FormStartPosition.CenterParent;
                    this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 9F);
                    this.Controls.AddRange(new Control[] { pnlInfo, btnPrivate, btnPublic });
                    pnlInfo.Controls.Add(lblInfo);
                    int a = 20.DpiZoom();
                    this.ClientSize = new Size(lblInfo.Width + 2 * a, lblInfo.Height + btnPrivate.Height + 3 * a);
                    lblInfo.Location = new Point(a, a);
                    pnlInfo.Height = lblInfo.Bottom + a;
                    btnPrivate.Top = btnPublic.Top = pnlInfo.Bottom + a / 2;
                    btnPublic.Left = pnlInfo.Width - btnPublic.Width - a;
                    btnPrivate.Left = btnPublic.Left - btnPrivate.Width - a;
                    btnPrivate.Click += (sender, e) => Mode = SubMode.Private;
                    btnPublic.Click += (sender, e) => Mode = SubMode.Public;
                }

                public enum SubMode { Public, Private, None }
                public SubMode Mode { get; private set; } = SubMode.None;

                readonly Label lblInfo = new Label
                {
                    Text = AppString.Dialog.SelectSubMenuMode,
                    AutoSize = true
                };
                readonly Panel pnlInfo = new Panel
                {
                    BackColor = Color.White,
                    Dock = DockStyle.Top
                };
                readonly Button btnPrivate = new Button
                {
                    Text = AppString.Dialog.Private,
                    DialogResult = DialogResult.OK,
                    AutoSize = true
                };
                readonly Button btnPublic = new Button
                {
                    Text = AppString.Dialog.Public,
                    DialogResult = DialogResult.OK,
                    AutoSize = true
                };
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            sealed class PulicMultiItemsList : MyList
            {
                readonly List<string> SubKeyNames = new List<string>();
                /// <summary>子菜单的父菜单的注册表路径</summary>
                private string ParentPath { get; set; }
                /// <summary>菜单所处环境注册表路径</summary>
                private string ScenePath => RegistryEx.GetParentPath(RegistryEx.GetParentPath(ParentPath));

                readonly SubNewItem subNewItem = new SubNewItem(true);

                public PulicMultiItemsList(MyListBox owner) : base(owner)
                {
                    this.AddItem(subNewItem);
                    subNewItem.AddNewItem += (sender, e) => AddNewItem();
                    subNewItem.AddExisting += (sender, e) => AddReference();
                    subNewItem.AddSeparator += (sender, e) => AddSeparator();
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
                    if(!SubShellTypeItem.CanAddMore(this)) return;
                    using(NewShellDialog dlg = new NewShellDialog())
                    {
                        dlg.ScenePath = this.ScenePath;
                        dlg.ShellPath = ShellItem.CommandStorePath;
                        if(dlg.ShowDialog() != DialogResult.OK) return;
                        SubKeyNames.Add(dlg.NewItemKeyName);
                        SaveSorting();
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
                            SaveSorting();
                        }
                    }
                }

                private void AddSeparator()
                {
                    this.SubKeyNames.Add("|");
                    SaveSorting();
                    this.AddItem(new SeparatorItem(this));
                }

                private void SaveSorting()
                {
                    SetValue(ParentPath, "SubCommands", string.Join(";", SubKeyNames.ToArray()));
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
                    this.Controls.Remove(item);
                    this.Controls[index - 1].Focus();
                    this.SubKeyNames.RemoveAt(index - 1);
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
                        if(MessageBoxEx.Show(AppString.Message.ConfirmDeleteReference,
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                        MyToolTip.SetToolTip(this, AppString.Tip.InvalidItem);
                        MyToolTip.SetToolTip(BtnDelete, AppString.Menu.Delete);
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

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            sealed class PrivateMultiItemsList : MyList
            {
                public PrivateMultiItemsList(MyListBox owner) : base(owner)
                {
                    this.AddItem(subNewItem);
                    subNewItem.AddNewItem += (sender, e) => AddNewItem();
                    subNewItem.AddSeparator += (sender, e) => AddSeparator();
                    subNewItem.AddExisting += (sender, e) => AddFromParentMenu();
                }

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

                public void LoadItems(string parentPath)
                {
                    this.ParentPath = parentPath;
                    string sckValue = GetValue(parentPath, "ExtendedSubCommandsKey", null)?.ToString();
                    if(!sckValue.IsNullOrWhiteSpace())
                    {
                        this.ShellPath = $@"{RegistryEx.CLASSESROOT}\{sckValue}\shell";
                    }
                    else
                    {
                        this.ShellPath = $@"{parentPath}\shell";
                    }
                    using(var shellKey = RegistryEx.GetRegistryKey(ShellPath))
                    {
                        if(shellKey == null) return;
                        RegTrustedInstaller.TakeRegTreeOwnerShip(shellKey.Name);
                        foreach(string keyName in shellKey.GetSubKeyNames())
                        {
                            string regPath = $@"{ShellPath}\{keyName}";
                            int value = Convert.ToInt32(GetValue(regPath, "CommandFlags", 0));
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
                    SetValue(regPath, "CommandFlags", 0x8);
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
                    MyToolTip.SetToolTip(BtnDelete, AppString.Menu.Delete);
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
                    if(!flag) MessageBoxEx.Show(AppString.Message.CannotAddNewItem);
                    return flag;
                }
            }

            sealed class SubNewItem : NewItem
            {
                public SubNewItem(bool isPublic)
                {
                    this.AddCtrs(new[] { btnAddExisting, btnAddSeparator });
                    MyToolTip.SetToolTip(btnAddExisting, isPublic ? AppString.Tip.AddReference : AppString.Tip.AddFromParentMenu);
                    MyToolTip.SetToolTip(btnAddSeparator, AppString.Tip.AddSeparator);
                    btnAddExisting.MouseDown += (sender, e) => AddExisting?.Invoke(null, null);
                    btnAddSeparator.MouseDown += (sender, e) => AddSeparator?.Invoke(null, null);
                }

                readonly PictureButton btnAddExisting = new PictureButton(AppImage.AddExisting);
                readonly PictureButton btnAddSeparator = new PictureButton(AppImage.AddSeparator);

                public event EventHandler AddExisting;
                public event EventHandler AddSeparator;
            }
        }
    }
}