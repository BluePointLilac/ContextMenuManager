using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using ContextMenuManager.Methods;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    /* 新建菜单项成立条件与相关规则:（恶心的关联方式，反复研究了好久）
     * 
     * 1.① 扩展名的关联打开方式（以下简称[OpenMode]，对应路径简称[OpenModePath]）
     *   ② HKCR默认值打开方式（以下简称[DefaultOpenMode]，对应路径简称[DefaultOpenModePath]）
     *   以上两个打开方式不一定相同
     * 
     * 2.① [DefaultOpenMode]不能为空，[DefaultOpenModePath]必须存在
     *   ② 菜单文本也不可为空
     *   ③ ShellNew项中必须存在 NullFile、Data、FileName、Directory、Command 中的一个或多个键值
     *   以上三个条件缺一不可，否则菜单不成立
     *   
     * 3.菜单名称取值优先级:
     *   ① ShellNew项的 MenuText 键值（必须为带@的资源文件字符串)
     *   ② [DefaultOpenModePath] 的 FriendlyTypeName 键值
     *   ③ [DefaultOpenModePath] 的默认键值
     *   ④ ②和③虽然不是第一优先级，但至少得存在一个，否则菜单不成立
     *   
     * 4.菜单图标取值优先级:
     *   ① ShellNew项的 IconPath 键值
     *   ② [OpenModePath]\DefaultIcon 的默认键值
     *   ③ 关联程序图标
     */

    sealed class ShellNewItem : MyListItem, IChkVisibleItem, ITsiTextItem, IBtnShowMenuItem, IBtnMoveUpDownItem,
         ITsiIconItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiRegDeleteItem, ITsiRegExportItem, ITsiCommandItem
    {
        public static readonly string[] SnParts = { "ShellNew", "-ShellNew" };
        public static readonly string[] UnableSortExtensions = { "Folder", ".library-ms" };
        public static readonly string[] DefaultBeforeSeparatorExtensions = { "Folder", ".library-ms", ".lnk" };
        public static readonly string[] EffectValueNames = { "NullFile", "Data", "FileName", "Directory", "Command" };
        private static readonly string[] UnableEditDataValues = { "Directory", "FileName", "Handler", "Command" };
        private static readonly string[] UnableChangeCommandValues = { "Data", "Directory", "FileName", "Handler" };

        public ShellNewItem(ShellNewList list, string regPath)
        {
            this.Owner = list;
            InitializeComponents();
            this.RegPath = regPath;
            SetSortabled(ShellNewList.ShellNewLockItem.IsLocked);
        }

        private string regPath;
        public string RegPath
        {
            get => regPath;
            set
            {
                regPath = value;
                this.Text = this.ItemText;
                this.Image = this.ItemIcon.ToBitmap();
            }
        }

        public string ValueName => null;
        public string SearchText => $"{AppString.SideBar.New} {Text}";
        public string Extension => RegPath.Split('\\')[1];
        private string SnKeyName => RegistryEx.GetKeyName(RegPath);
        private string BackupPath => $@"{RegistryEx.GetParentPath(RegPath)}\{(ItemVisible ? SnParts[1] : SnParts[0])}";
        private string OpenMode => FileExtension.GetOpenMode(Extension);//关联打开方式
        private string OpenModePath => $@"{RegistryEx.CLASSES_ROOT}\{OpenMode}";//关联打开方式注册表路径
        private string DefaultOpenMode => Registry.GetValue($@"{RegistryEx.CLASSES_ROOT}\{Extension}", "", null)?.ToString();//HKCR默认值打开方式
        private string DefaultOpenModePath => $@"{RegistryEx.CLASSES_ROOT}\{DefaultOpenMode}";//HKCR默认值打开方式路径
        public bool CanSort => !UnableSortExtensions.Contains(Extension, StringComparer.OrdinalIgnoreCase);//能够排序的
        private bool CanEditData => UnableEditDataValues.All(value => Registry.GetValue(RegPath, value, null) == null);//能够编辑初始数据的
        private bool CanChangeCommand => UnableChangeCommandValues.All(value => Registry.GetValue(RegPath, value, null) == null);//能够更改菜单命令的
        private bool DefaultBeforeSeparator => DefaultBeforeSeparatorExtensions.Contains(Extension, StringComparer.OrdinalIgnoreCase);//默认显示在分割线上不可更改的

        public string ItemFilePath
        {
            get
            {
                string filePath = FileExtension.GetExtentionInfo(FileExtension.AssocStr.Executable, Extension);
                if(File.Exists(filePath)) return filePath;
                using(RegistryKey oKey = RegistryEx.GetRegistryKey(OpenModePath))
                {
                    using(RegistryKey aKey = oKey.OpenSubKey("Application"))
                    {
                        string uwp = aKey?.GetValue("AppUserModelID")?.ToString();
                        if(uwp != null) return "shell:AppsFolder\\" + uwp;
                    }
                    using(RegistryKey cKey = oKey.OpenSubKey("CLSID"))
                    {
                        string value = cKey?.GetValue("")?.ToString();
                        if(GuidEx.TryParse(value, out Guid guid))
                        {
                            filePath = GuidInfo.GetFilePath(guid);
                            if(filePath != null) return filePath;
                        }
                    }
                }
                return null;
            }
        }

        public bool ItemVisible
        {
            get => SnKeyName.Equals(SnParts[0], StringComparison.OrdinalIgnoreCase);
            set
            {
                RegistryEx.MoveTo(RegPath, BackupPath);
                this.RegPath = BackupPath;
            }
        }

        public string ItemText
        {
            get
            {
                string name = Registry.GetValue(RegPath, "MenuText", null)?.ToString();
                if(name!=null&&name.StartsWith("@"))
                {
                    name = ResourceString.GetDirectString(name);
                    if(!string.IsNullOrEmpty(name)) return name;
                }
                name = Registry.GetValue(DefaultOpenModePath, "FriendlyTypeName", null)?.ToString();
                name = ResourceString.GetDirectString(name);
                if(!string.IsNullOrEmpty(name)) return name;
                name = Registry.GetValue(DefaultOpenModePath, "", null)?.ToString();
                if(!string.IsNullOrEmpty(name)) return name;
                return null;
            }
            set
            {
                RegistryEx.DeleteValue(RegPath, "MenuText");
                Registry.SetValue(DefaultOpenModePath, "FriendlyTypeName", value);
                this.Text = ResourceString.GetDirectString(value);
            }
        }

        public string IconLocation
        {
            get
            {
                string value = Registry.GetValue(RegPath, "IconPath", null)?.ToString();
                if(!value.IsNullOrWhiteSpace()) return value;
                value = Registry.GetValue($@"{OpenModePath}\DefaultIcon", "", null)?.ToString();
                if(!value.IsNullOrWhiteSpace()) return value;
                return ItemFilePath;
            }
            set => Registry.SetValue(RegPath, "IconPath", value);
        }

        public Icon ItemIcon
        {
            get
            {
                string location = IconLocation;
                if(location == null || location.StartsWith("@"))
                {
                    return ResourceIcon.GetExtensionIcon(Extension);
                } 
                Icon icon = ResourceIcon.GetIcon(location, out string path, out int index);
                if(icon == null) icon = ResourceIcon.GetIcon(path = "imageres.dll", index = -2);
                IconPath = path; IconIndex = index;
                return icon;
            }
        }

        public string IconPath { get; set; }
        public int IconIndex { get; set; }

        private object InitialData
        {
            get => Registry.GetValue(RegPath, "Data", null);
            set => Registry.SetValue(RegPath, "Data", value);
        }

        public string ItemCommand
        {
            get => Registry.GetValue(RegPath, "Command", null)?.ToString();
            set
            {
                if(value.IsNullOrWhiteSpace())
                {
                    if(Registry.GetValue(RegPath, "NullFile", null) != null)
                    {
                        RegistryEx.DeleteValue(RegPath, "Command");
                    }
                }
                else
                {
                    Registry.SetValue(RegPath, "Command", value);
                }
            }
        }

        public bool BeforeSeparator
        {
            get
            {
                if(DefaultBeforeSeparator) return true;
                else return Registry.GetValue($@"{RegPath}\Config", "BeforeSeparator", null) != null;
            }
            set
            {
                if(value)
                {
                    Registry.SetValue($@"{RegPath}\Config", "BeforeSeparator", "");
                }
                else
                {
                    using(RegistryKey snkey = RegistryEx.GetRegistryKey(RegPath, true))
                    using(RegistryKey ckey = snkey.OpenSubKey("Config", true))
                    {
                        ckey.DeleteValue("BeforeSeparator");
                        if(ckey.GetValueNames().Length == 0 && ckey.GetSubKeyNames().Length == 0)
                        {
                            snkey.DeleteSubKey("Config");
                        }
                    }
                }
            }
        }

        public ShellNewList Owner { get; private set; }
        public MoveButton BtnMoveUp { get; set; }
        public MoveButton BtnMoveDown { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        public ChangeIconMenuItem TsiChangeIcon { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        public RegExportMenuItem TsiRegExport { get; set; }
        public ChangeCommandMenuItem TsiChangeCommand { get; set; }

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);
        readonly ToolStripMenuItem TsiOtherAttributes = new ToolStripMenuItem(AppString.Menu.OtherAttributes);
        readonly ToolStripMenuItem TsiBeforeSeparator = new ToolStripMenuItem(AppString.Menu.BeforeSeparator);
        readonly ToolStripMenuItem TsiEditData = new ToolStripMenuItem(AppString.Menu.InitialData);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            BtnMoveDown = new MoveButton(this, false);
            BtnMoveUp = new MoveButton(this, true);
            TsiSearch = new WebSearchMenuItem(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiChangeIcon = new ChangeIconMenuItem(this);
            TsiChangeCommand = new ChangeCommandMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiRegExport = new RegExportMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);
            TsiChangeCommand.CommandCanBeEmpty = true;

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] {TsiChangeText,
                new ToolStripSeparator(), TsiChangeIcon, new ToolStripSeparator(), TsiOtherAttributes,
                new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiOtherAttributes.DropDownItems.AddRange(new[] { TsiBeforeSeparator, TsiEditData });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch,
                new ToolStripSeparator(), TsiChangeCommand, TsiFileProperties,
                TsiFileLocation, TsiRegLocation, TsiRegExport });

            ContextMenuStrip.Opening += (sender, e) =>
            {
                TsiEditData.Visible = CanEditData;
                TsiChangeCommand.Visible = CanChangeCommand;
                TsiBeforeSeparator.Enabled = !DefaultBeforeSeparator;
                TsiBeforeSeparator.Checked = BeforeSeparator;
            };
            TsiEditData.Click += (sender, e) => EditInitialData();
            TsiBeforeSeparator.Click += (sender, e) => MoveWithSeparator(!TsiBeforeSeparator.Checked);
            BtnMoveUp.MouseDown += (sender, e) => Owner.MoveItem(this, true);
            BtnMoveDown.MouseDown += (sender, e) => Owner.MoveItem(this, false);
        }

        private void EditInitialData()
        {
            if(AppMessageBox.Show(AppString.Message.EditInitialData,
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            using(InputDialog dlg = new InputDialog
            {
                Title = AppString.Menu.InitialData,
                Text = this.InitialData?.ToString()
            })
            {
                if(dlg.ShowDialog() == DialogResult.OK) this.InitialData = dlg.Text;
            }
        }

        public void SetSortabled(bool isLocked)
        {
            BtnMoveDown.Visible = BtnMoveUp.Visible = isLocked && CanSort;
        }

        private void MoveWithSeparator(bool isBefore)
        {
            BeforeSeparator = isBefore;
            ShellNewList list = (ShellNewList)this.Parent;
            int index = list.GetItemIndex(list.Separator);
            list.SetItemIndex(this, index);
            if(ShellNewList.ShellNewLockItem.IsLocked) list.SaveSorting();
        }

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            RegistryEx.DeleteKeyTree(this.BackupPath);
            this.Parent.Controls.Remove(this);
            if(ShellNewList.ShellNewLockItem.IsLocked) Owner.SaveSorting();
        }
    }
}