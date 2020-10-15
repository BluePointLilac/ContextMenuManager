using BulePointLilac.Controls;
using BulePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class ShellItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiTextItem,
        ITsiCommandItem, ITsiIconItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiDeleteItem
    {
        /// <summary>Shell子菜单注册表项路径</summary>
        public const string CommandStorePath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell";

        /// <summary>Shell类型菜单特殊注册表项名默认名称</summary>
        private static readonly Dictionary<string, string> DefaultNames
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            {"open", AppString.Open }, {"edit", AppString.Edit }, {"print", AppString.Print },
            {"find", AppString.Find }, {"play", AppString.Play }, {"runas", AppString.Runas },
            {"explore", AppString.Text_Explore },//"浏览" 未找到合适的本地化字符串资源
        };

        /// <summary>菜单项目在菜单中出现的位置</summary>
        enum Positions { Default, Top, Bottom }

        public ShellItem(string regPath)
        {
            InitializeComponents();
            this.RegPath = regPath;
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
                if(!HasIcon) this.Image = Image.ToTransparent();
                ChkVisible.Checked = this.ItemVisible;
                BtnSubItems.Visible = IsMultiItem;
            }
        }

        public string SearchText => Text;
        private string CommandPath => $@"{RegPath}\command";
        public string KeyName => RegistryEx.GetKeyName(RegPath);
        private bool IsMultiItem => Registry.GetValue(RegPath, "SubCommands", null) != null;
        protected virtual bool IsSubItem => false;
        private bool IsOpenItem => KeyName.ToLower() == "open";
        private bool TryProtectOpenItem => IsOpenItem && MessageBoxEx.Show(AppString.MessageBox_PromptIsOpenItem,
                MessageBoxButtons.YesNo) != DialogResult.Yes;

        public string ItemFilePath => GuidInfo.GetFilePath(Guid) ?? ObjectPath.ExtractFilePath(ItemCommand);
        private bool HasIcon => IconLocation != null || HasLUAShield;

        private bool OnlyInExplorer
        {
            get => Registry.GetValue(RegPath, "OnlyInBrowserWindow", null) != null;
            set
            {
                if(value)
                {
                    if(TryProtectOpenItem) return;
                    Registry.SetValue(RegPath, "OnlyInBrowserWindow", "");
                }
                else RegistryEx.DeleteValue(RegPath, "OnlyInBrowserWindow");
            }
        }

        private bool OnlyWithShift
        {
            get => Registry.GetValue(RegPath, "Extended", null) != null;
            set
            {
                if(value)
                {
                    if(TryProtectOpenItem) return;
                    Registry.SetValue(RegPath, "Extended", "");
                }
                else RegistryEx.DeleteValue(RegPath, "Extended");
            }
        }

        private bool NoWorkingDirectory
        {
            get => Registry.GetValue(RegPath, "NoWorkingDirectory", null) != null;
            set
            {
                if(value) Registry.SetValue(RegPath, "NoWorkingDirectory", "");
                else RegistryEx.DeleteValue(RegPath, "NoWorkingDirectory");
            }
        }

        private Positions ItemPosition
        {
            get
            {
                string value = Registry.GetValue(RegPath, "Position", null)?.ToString()?.ToLower();
                switch(value)
                {
                    case "top":
                        return Positions.Top;
                    case "bottom":
                        return Positions.Bottom;
                    default:
                        return Positions.Default;
                }
            }
            set
            {
                switch(value)
                {
                    case Positions.Top:
                        Registry.SetValue(RegPath, "Position", "top");
                        break;
                    case Positions.Bottom:
                        Registry.SetValue(RegPath, "Position", "bottom");
                        break;
                    case Positions.Default:
                        RegistryEx.DeleteValue(RegPath, "Position");
                        break;
                }
            }
        }

        public bool ItemVisible
        {
            get
            {
                if(Convert.ToInt32(Registry.GetValue(RegPath, "HideBasedOnVelocityId", null)) == 0x639bc8) return false;
                if(!IsSubItem)
                {
                    if(Registry.GetValue(RegPath, "LegacyDisable", null) != null) return false;
                    if(Registry.GetValue(RegPath, "ProgrammaticAccessOnly", null) != null) return false;
                }
                return true;
            }
            set
            {
                if(value)
                {
                    RegistryEx.DeleteValue(RegPath, "HideBasedOnVelocityId");
                    RegistryEx.DeleteValue(RegPath, "LegacyDisable");
                    RegistryEx.DeleteValue(RegPath, "ProgrammaticAccessOnly");
                }
                else
                {
                    if(TryProtectOpenItem) return;
                    Registry.SetValue(RegPath, "HideBasedOnVelocityId", 0x639bc8);
                }
            }
        }

        public string ItemText
        {
            get
            {
                string name;
                //菜单名称优先级别：MUIVerb > 默认值 > 特殊键值名 > 项名
                List<string> valueNames = new List<string> { "MUIVerb" };
                if(!IsMultiItem) valueNames.Add("");//多级母菜单不支持使用默认值作为名称
                foreach(string valueName in valueNames)
                {
                    name = Registry.GetValue(RegPath, valueName, null)?.ToString();
                    name = ResourceString.GetDirectString(name);
                    if(!string.IsNullOrEmpty(name)) return name;
                }
                if(DefaultNames.TryGetValue(KeyName, out name)) return name;
                else return KeyName;
            }
            set
            {
                //MUIVerb长度不可超过80,超过80系统会隐藏该菜单项目
                if(ResourceString.GetDirectString(value).Length >= 80)
                    MessageBoxEx.Show(AppString.MessageBox_TextLengthCannotExceed80);
                else Registry.SetValue(RegPath, "MUIVerb", value);
            }
        }

        public string ItemCommand
        {
            get
            {
                if(IsMultiItem) return null;
                else return Registry.GetValue(CommandPath, "", null)?.ToString();
            }
            set
            {
                if(TryProtectOpenItem) return;
                Registry.SetValue(CommandPath, "", value);
                if(!this.HasIcon) this.Image = this.ItemIcon.ToBitmap().ToTransparent();
            }
        }

        private bool HasLUAShield
        {
            get => Registry.GetValue(RegPath, "HasLUAShield", null) != null;
            set
            {
                if(value) Registry.SetValue(RegPath, "HasLUAShield", "");
                else RegistryEx.DeleteValue(RegPath, "HasLUAShield");
            }
        }

        public string IconLocation
        {
            get => Registry.GetValue(RegPath, "Icon", null)?.ToString();
            set
            {
                if(value != null) Registry.SetValue(RegPath, "Icon", value);
                else RegistryEx.DeleteValue(RegPath, "Icon");
            }
        }

        public string IconPath { get; set; }
        public int IconIndex { get; set; }
        public Icon ItemIcon
        {
            get
            {
                //菜单图标优先级别：Icon > HasLUAShield
                //只要有Icon键值，不论数据是否为空，HasLUAShield键值就不起作用
                Icon icon;
                string iconPath;
                int iconIndex;
                if(IconLocation != null)
                {
                    icon = ResourceIcon.GetIcon(IconLocation, out iconPath, out iconIndex);
                    if(icon == null && Path.GetExtension(iconPath).ToLower() == ".exe")
                        icon = ResourceIcon.GetIcon(iconPath = "imageres.dll", iconIndex = 11);//文件为不存在的或没有图标的exe文件，不含图标的默认exe图标
                }
                else if(HasLUAShield)
                    icon = ResourceIcon.GetIcon(iconPath = "imageres.dll", iconIndex = 73);//管理员小盾牌图标
                else icon = ResourceIcon.GetIcon(iconPath = ItemFilePath, iconIndex = 0);//文件第一个图标
                if(icon == null) icon = ResourceIcon.GetIcon(iconPath = "imageres.dll", iconIndex = 2);//图标资源不存在，白纸图标
                IconPath = iconPath;
                IconIndex = iconIndex;
                return icon;
            }
        }

        private Guid Guid
        {
            get
            {
                string value = Registry.GetValue(CommandPath, "DelegateExecute", null)?.ToString();
                if(GuidInfo.TryGetGuid(value, out Guid guid)) return guid;
                else return Guid.Empty;
            }
        }

        public VisibleCheckBox ChkVisible { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        public ChangeIconMenuItem TsiChangeIcon { get; set; }
        public ChangeCommandMenuItem TsiChangeCommand { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }

        protected readonly ToolStripMenuItem TsiOtherAttributes = new ToolStripMenuItem(AppString.Menu_OtherAttributes);
        readonly ToolStripMenuItem TsiItemIcon = new ToolStripMenuItem(AppString.Menu_ItemIcon);
        readonly ToolStripMenuItem TsiDeleteIcon = new ToolStripMenuItem(AppString.Menu_DeleteIcon);
        readonly ToolStripMenuItem TsiPosition = new ToolStripMenuItem(AppString.Menu_ItemPosition);
        readonly ToolStripMenuItem TsiDefault = new ToolStripMenuItem(AppString.Menu_SetDefault);
        readonly ToolStripMenuItem TsiTop = new ToolStripMenuItem(AppString.Menu_SetTop);
        readonly ToolStripMenuItem TsiBottom = new ToolStripMenuItem(AppString.Menu_SetBottom);
        readonly ToolStripMenuItem TsiShift = new ToolStripMenuItem(AppString.Menu_OnlyWithShift);
        readonly ToolStripMenuItem TsiExplorer = new ToolStripMenuItem(AppString.Menu_OnlyInExplorer);
        readonly ToolStripMenuItem TsiNoWorkDir = new ToolStripMenuItem(AppString.Menu_NoWorkingDirectory);
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu_Details);
        readonly PictureButton BtnSubItems = new PictureButton(AppImage.SubItems);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiChangeCommand = new ChangeCommandMenuItem(this);
            TsiChangeIcon = new ChangeIconMenuItem(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText, new ToolStripSeparator(), TsiItemIcon,
                TsiPosition, TsiOtherAttributes, new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe});

            TsiItemIcon.DropDownItems.AddRange(new ToolStripItem[] { TsiChangeIcon, TsiDeleteIcon });

            TsiPosition.DropDownItems.AddRange(new ToolStripItem[] { TsiDefault, TsiTop, TsiBottom });

            TsiOtherAttributes.DropDownItems.AddRange(new ToolStripItem[] { TsiShift, TsiExplorer, TsiNoWorkDir });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiChangeCommand, TsiFileProperties, TsiFileLocation, TsiRegLocation});

            TsiDeleteIcon.Click += (sender, e) => DeleteIcon();
            TsiTop.Click += (sender, e) => this.ItemPosition = Positions.Top;
            TsiBottom.Click += (sender, e) => this.ItemPosition = Positions.Bottom;
            TsiDefault.Click += (sender, e) => this.ItemPosition = Positions.Default;
            TsiExplorer.Click += (sender, e) => this.OnlyInExplorer = !TsiExplorer.Checked;
            TsiShift.Click += (sender, e) => this.OnlyWithShift = !TsiShift.Checked;
            TsiNoWorkDir.Click += (sender, e) => this.NoWorkingDirectory = !TsiNoWorkDir.Checked;
            ContextMenuStrip.Opening += (sender, e) => RefreshMenuItem();
            BtnSubItems.MouseDown += (sender, e) => ShowSubItems();
            MyToolTip.SetToolTip(BtnSubItems, AppString.Tip_EditSubItems);
            this.AddCtr(BtnSubItems);
        }

        private void DeleteIcon()
        {
            this.IconLocation = null;
            this.HasLUAShield = false;
            this.Image = this.Image.ToTransparent();
        }

        private void RefreshMenuItem()
        {
            if(this.HasIcon)
            {
                TsiChangeIcon.Text = AppString.Menu_ChangeIcon;
                TsiDeleteIcon.Visible = true;
            }
            else
            {
                TsiChangeIcon.Text = AppString.Menu_AddIcon;
                TsiDeleteIcon.Visible = false;
            }
            TsiDeleteMe.Enabled = !IsOpenItem;
            TsiChangeCommand.Visible = !IsMultiItem && Guid.Equals(Guid.Empty);
            TsiPosition.Visible = TsiExplorer.Visible = TsiShift.Visible = !IsSubItem;
            TsiNoWorkDir.Checked = this.NoWorkingDirectory;
            if(!this.IsSubItem)
            {
                TsiShift.Checked = this.OnlyWithShift;
                TsiExplorer.Checked = this.OnlyInExplorer;
                TsiDefault.Checked = TsiTop.Checked = TsiBottom.Checked = false;
                switch(this.ItemPosition)
                {
                    case Positions.Default:
                        TsiDefault.Checked = true;
                        break;
                    case Positions.Top:
                        TsiTop.Checked = true;
                        break;
                    case Positions.Bottom:
                        TsiBottom.Checked = true;
                        break;
                }
            }
        }

        private void ShowSubItems()
        {
            using(ShellSubMenuDialog dlg = new ShellSubMenuDialog())
            {
                dlg.Text = AppString.Text_EditSubItems.Replace("%s", this.Text);
                dlg.Icon = ResourceIcon.GetIcon(IconPath, IconIndex);
                dlg.ShowDialog(this.RegPath);
            }
        }

        public virtual void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            this.Dispose();
        }
    }
}