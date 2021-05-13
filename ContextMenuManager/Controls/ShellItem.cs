using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class ShellItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiTextItem, ITsiCommandItem,
        ITsiIconItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiRegDeleteItem, ITsiRegExportItem
    {
        /// <summary>Shell公共引用子菜单注册表项路径</summary>
        public const string CommandStorePath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell";

        private const string OpenInNewWindowPath = @"HKEY_CLASSES_ROOT\Folder\shell\opennewwindow";

        /// <summary>系统原有Shell公共子菜单项名</summary>
        public static readonly string[] SysStoreItemNames = { "Windows.aboutWindows", "Windows.AddColumns",
                "Windows.AddDevice", "Windows.AddMediaServer", "Windows.AddNetworkLocation", "Windows.AddPrinter",
                "Windows.AddRemovePrograms", "Windows.AddToFavorites", "Windows.Autoplay", "Windows.Backup",
                "Windows.BitLocker", "Windows.BitLocker.Encrypt", "Windows.BitLocker.Manage",
                "Windows.BitLocker.ResetPasswordPin", "Windows.burn", "Windows.Burn.Action", "Windows.change-passphrase",
                "Windows.change-pin", "Windows.ChangeIndexedLocations", "Windows.ChooseColumns", "Windows.CleanUp",
                "Windows.ClearAddressBarHistory", "Windows.ClearFrequentHistory", "Windows.clearRecentDocs",
                "Windows.closewindow", "Windows.cmd", "Windows.cmdPromptAsAdministrator", "Windows.CompressedFile.extract",
                "Windows.CompressedFile.ExtractTo", "Windows.CompressedFolder.extract", "Windows.CompressedItem.extract",
                "Windows.Computer.Manage", "Windows.connectNetworkDrive", "Windows.copy", "Windows.copyaspath",
                "Windows.CopyToBrowser", "Windows.CopyToMenu", "Windows.CscSync", "Windows.CscWorkOfflineOnline",
                "Windows.cut", "Windows.Defragment", "Windows.delete", "Windows.Dialog.DisconnectNetworkDrive",
                "Windows.DiscImage.burn", "Windows.DisconnectNetworkDrive", "Windows.DiskFormat",
                "Windows.DriveFolder.DisconnectNetworkDrive", "Windows.edit", "Windows.Eject", "Windows.email",
                "Windows.encrypt-bde", "Windows.encrypt-bde-elev", "Windows.Enqueue", "Windows.EraseDisc",
                "Windows.EraseDisc.Action", "Windows.fax", "Windows.FinishBurn", "Windows.folderoptions",
                "Windows.GroupByColumn", "Windows.help", "Windows.HideSelected", "Windows.HistoryVaultRestore",
                "Windows.HomeGroupCPL", "Windows.HomeGroupJoin", "Windows.HomeGroupPassword", "Windows.HomeGroupSharing",
                "Windows.HomeGroupTroubleshooter", "Windows.IconSize", "Windows.includeinlibrary", "Windows.invertselection",
                "Windows.layout", "Windows.LibraryChangeIcon", "Windows.LibraryDefaultSaveLocation",
                "Windows.LibraryIncludeInLibrary", "Windows.LibraryManageLibrary", "Windows.LibraryOptimizeLibraryFor",
                "Windows.LibraryPublicSaveLocation", "Windows.LibraryRestoreDefaults", "Windows.LibrarySelChangeIcon",
                "Windows.LibrarySelDefaultSaveLocation", "Windows.LibrarySelManageLibrary", "Windows.LibrarySelOptimizeLibraryFor",
                "Windows.LibrarySelPublicSaveLocation", "Windows.LibrarySelRestoreDefaults", "Windows.LibrarySelShowInNavPane",
                "Windows.LibraryShowInNavPane", "Windows.location.cmd", "Windows.location.cmdPromptAsAdministrator",
                "Windows.location.opennewprocess", "Windows.location.opennewtab", "Windows.location.opennewwindow",
                "Windows.location.Powershell", "Windows.location.PowershellAsAdmin", "Windows.manage-bde",
                "Windows.manage-bde-elev", "Windows.MapNetworkDrive", "Windows.menubar", "Windows.ModernShare",
                "Windows.mount", "Windows.MoveToBrowser", "Windows.MoveToMenu", "Windows.MultiVerb.cmd",
                "Windows.MultiVerb.cmdPromptAsAdministrator", "Windows.MultiVerb.opennewprocess", "Windows.MultiVerb.opennewtab",
                "Windows.MultiVerb.opennewwindow", "Windows.MultiVerb.Powershell", "Windows.MultiVerb.PowershellAsAdmin",
                "Windows.navpane", "Windows.NavPaneExpandToCurrentFolder", "Windows.NavPaneShowAllFolders",
                "Windows.NavPaneShowLibraries", "Windows.NetworkAndSharing", "Windows.NetworkViewDeviceWebpage",
                "Windows.newfolder", "Windows.newitem", "Windows.open", "Windows.OpenContainingFolder.opencontaining",
                "Windows.OpenControlPanel", "Windows.opennewprocess", "Windows.opennewtab", "Windows.opennewwindow",
                "Windows.OpenPrinterServerProperty", "Windows.OpenPrintQueue", "Windows.OpenSearch.openfilelocation",
                "Windows.OpenSearchViewSite", "Windows.OpenWith", "Windows.organize", "Windows.paste", "Windows.pastelink",
                "Windows.PermanentDelete", "Windows.PinToHome", "Windows.pintostartscreen", "Windows.play", "Windows.playall",
                "Windows.playmusic", "Windows.PowershellAsAdmin", "Windows.previewpane", "Windows.print", "Windows.properties",
                "Windows.readingpane", "Windows.recycle", "Windows.RecycleBin.Empty", "Windows.RecycleBin.Location.properties",
                "Windows.RecycleBin.properties", "Windows.RecycleBin.RestoreAll", "Windows.RecycleBin.RestoreItems",
                "Windows.RecycleBin.Selection.properties", "Windows.redo", "Windows.remotedesktop", "Windows.RemoveMediaServer",
                "Windows.removeproperties", "Windows.rename", "Windows.RibbonDelete", "Windows.RibbonPermissionsDialog",
                "Windows.RibbonShare", "Windows.RibbonSync.MakeAvailableOffline", "Windows.RibbonSync.SyncThisFolder",
                "Windows.RibbonSync.WorkOfflineOnline", "Windows.rotate270", "Windows.rotate90", "Windows.runas",
                "Windows.runasuser", "Windows.SearchActiveDirectory", "Windows.SearchClearMru", "Windows.SearchCloseTab",
                "Windows.SearchFilterDate", "Windows.SearchFilterKind", "Windows.SearchFilterMoreProperties",
                "Windows.SearchFilterSize", "Windows.SearchMru", "Windows.SearchOpenLocation", "Windows.SearchOptionCompressed",
                "Windows.SearchOptionContents", "Windows.SearchOptionDeep", "Windows.SearchOptionShallow",
                "Windows.SearchOptionSystem", "Windows.SearchSave", "Windows.SearchSendTo", "Windows.SearchSendToComputer",
                "Windows.selectall", "Windows.SelectionCheckboxes", "Windows.selectMode", "Windows.selectnone",
                "Windows.separator", "Windows.setdesktopwallpaper", "Windows.Share", "Windows.SharePrivate",
                "Windows.ShareSpecificUsers", "Windows.Shortcut.opencontaining", "Windows.ShowFileExtensions",
                "Windows.ShowHiddenFiles", "Windows.SizeAllColumns", "Windows.slideshow", "Windows.SortAscending",
                "Windows.SortByColumn", "Windows.SortDescending", "Windows.SortGroupsAscending", "Windows.SortGroupsDescending",
                "Windows.StartScan", "Windows.statusbar", "Windows.Sync", "Windows.SystemProperties", "Windows.taskbarpin",
                "Windows.ToggleRecycleConfirmations", "Windows.topviewrestoredefault", "Windows.Troubleshoot", "Windows.undo",
                "Windows.UpdatePrinterDriver", "Windows.v2.Powershell", "Windows.View.OptionsGallery",
                "Windows.ViewRemotePrinters", "Windows.zip", "Windows.Zip.Action",
                
                //Win8.1
                "Windows.ManageDefaultPrinters", "Windows.NavPaneShowFavorites", "Windows.playto", "Windows.Powershell",
                "Windows.ShareHomegroupFullAccess", "Windows.ShareHomegroupNoAccess", "Windows.ShareHomegroupReadAccess",
                //Win8
                "Windows.SearchOptionPartial",
                //Win7
                "Windows.librarypane"
            };

        /// <summary>Shell类型菜单特殊注册表项名默认名称</summary>
        private static readonly Dictionary<string, string> DefaultNames
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            {"open", AppString.Other.Open }, {"edit", AppString.Other.Edit }, {"print", AppString.Other.Print },
            {"find", AppString.Other.Find }, {"play", AppString.Other.Play }, {"runas", AppString.Other.Runas },
            //Win10为“浏览(&X)”，Win10之前为“资源管理器(&X)”
            {"explore", WindowsOsVersion.IsBefore10 ? AppString.Other.ExploreOld : AppString.Other.Explore }
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

        public string ValueName => null;
        public string SearchText => Text;
        private string CommandPath => $@"{RegPath}\command";
        public string KeyName => RegistryEx.GetKeyName(RegPath);
        protected virtual bool IsSubItem => false;
        private bool IsOpenItem => KeyName.ToLower() == "open";

        public string ItemFilePath => GuidInfo.GetFilePath(Guid) ?? ObjectPath.ExtractFilePath(ItemCommand);
        private bool HasIcon => !IconLocation.IsNullOrWhiteSpace() || HasLUAShield;

        private bool IsMultiItem
        {
            get
            {
                object value = Registry.GetValue(RegPath, "SubCommands", null);
                if(value != null) return true;
                value = Registry.GetValue(RegPath, "ExtendedSubCommandsKey", null);
                if(!string.IsNullOrEmpty(value?.ToString())) return true;
                return false;
            }
        }

        private bool OnlyInExplorer
        {
            get => Registry.GetValue(RegPath, "OnlyInBrowserWindow", null) != null;
            set
            {
                if(value)
                {
                    if(TryProtectOpenItem()) return;
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
                    if(TryProtectOpenItem()) return;
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

        private bool NeverDefault
        {
            get => Registry.GetValue(RegPath, "NeverDefault", null) != null;
            set
            {
                if(value) Registry.SetValue(RegPath, "NeverDefault", "");
                else RegistryEx.DeleteValue(RegPath, "NeverDefault");
            }
        }

        private bool ShowAsDisabledIfHidden
        {
            get => Registry.GetValue(RegPath, "ShowAsDisabledIfHidden", null) != null;
            set
            {
                if(value) Registry.SetValue(RegPath, "ShowAsDisabledIfHidden", "");
                else RegistryEx.DeleteValue(RegPath, "ShowAsDisabledIfHidden");
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
                if(WindowsOsVersion.IsAfterOrEqualWin10_1703)
                {
                    //HideBasedOnVelocityId键值仅适用于Win10系统1703以上版本
                    if(Convert.ToInt32(Registry.GetValue(RegPath, "HideBasedOnVelocityId", 0)) == 0x639bc8) return false;
                }
                if(!IsSubItem)
                {
                    //LegacyDisable和ProgrammaticAccessOnly键值不适用于子菜单
                    if(Registry.GetValue(RegPath, "LegacyDisable", null) != null) return false;
                    if(Registry.GetValue(RegPath, "ProgrammaticAccessOnly", null) != null) return false;
                    //CommandFlags键值不适用于Vista系统，子菜单中该键值我用来做分割线键值
                    if(WindowsOsVersion.IsAfterVista && Convert.ToInt32(Registry.GetValue(RegPath, "CommandFlags", 0)) % 16 >= 8) return false;
                }
                return true;
            }
            set
            {
                try
                {
                    void DeleteSomeValues()
                    {
                        RegistryEx.DeleteValue(RegPath, "LegacyDisable");
                        RegistryEx.DeleteValue(RegPath, "ProgrammaticAccessOnly");
                        if(WindowsOsVersion.IsAfterVista && Convert.ToInt32(Registry.GetValue(RegPath, "CommandFlags", 0)) % 16 >= 8)
                        {
                            RegistryEx.DeleteValue(RegPath, "CommandFlags");
                        }
                    };

                    if(value)
                    {
                        RegistryEx.DeleteValue(RegPath, "HideBasedOnVelocityId");
                        DeleteSomeValues();
                    }
                    else
                    {
                        if(TryProtectOpenItem()) return;
                        if(WindowsOsVersion.IsAfterOrEqualWin10_1703)
                        {
                            Registry.SetValue(RegPath, "HideBasedOnVelocityId", 0x639bc8);
                        }
                        else
                        {
                            if(IsSubItem)
                            {
                                MessageBoxEx.Show(AppString.Message.CannotHideSubItem);
                                return;
                            }
                        }
                        if(!IsSubItem)
                        {
                            //当LegaryDisable键值作用于文件夹-"在新窗口中打开"时
                            //会导致点击任务栏explorer图标和 Win+E 快捷键错误访问
                            if(!RegPath.StartsWith(OpenInNewWindowPath, StringComparison.OrdinalIgnoreCase))
                            {
                                Registry.SetValue(RegPath, "LegacyDisable", "");
                            }
                            Registry.SetValue(RegPath, "ProgrammaticAccessOnly", "");
                        }
                        if(ShowAsDisabledIfHidden) DeleteSomeValues();
                    }
                }
                catch
                {
                    MessageBoxEx.Show(AppString.Message.AuthorityProtection);
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
                {
                    MessageBoxEx.Show(AppString.Message.TextLengthCannotExceed80);
                }
                else
                {
                    Registry.SetValue(RegPath, "MUIVerb", value);
                    this.Text = ResourceString.GetDirectString(value);
                }
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
                if(TryProtectOpenItem()) return;
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
                    if(icon == null && Path.GetExtension(iconPath)?.ToLower() == ".exe")
                        icon = ResourceIcon.GetIcon(iconPath = "imageres.dll", iconIndex = -15);//文件为不存在的或没有图标的exe文件，不含图标的默认exe图标
                }
                else if(HasLUAShield)
                    icon = ResourceIcon.GetIcon(iconPath = "imageres.dll", iconIndex = -78);//管理员小盾牌图标
                else icon = ResourceIcon.GetIcon(iconPath = ItemFilePath, iconIndex = 0);//文件第一个图标
                if(icon == null) icon = ResourceIcon.GetIcon(iconPath = "imageres.dll", iconIndex = -2);//图标资源不存在，白纸图标
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
                if(!GuidEx.TryParse(value, out Guid guid))
                {
                    value = Registry.GetValue($@"{RegPath}\DropTarget", "CLSID", null)?.ToString();
                    GuidEx.TryParse(value, out guid);
                }
                return guid;
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
        public RegExportMenuItem TsiRegExport { get; set; }

        protected readonly PictureButton BtnSubItems = new PictureButton(AppImage.SubItems);
        protected readonly ToolStripMenuItem TsiOtherAttributes = new ToolStripMenuItem(AppString.Menu.OtherAttributes);
        readonly ToolStripMenuItem TsiItemIcon = new ToolStripMenuItem(AppString.Menu.ItemIcon);
        readonly ToolStripMenuItem TsiDeleteIcon = new ToolStripMenuItem(AppString.Menu.DeleteIcon);
        readonly ToolStripMenuItem TsiShieldIcon = new ToolStripMenuItem(AppString.Menu.ShieldIcon);
        readonly ToolStripMenuItem TsiPosition = new ToolStripMenuItem(AppString.Menu.ItemPosition);
        readonly ToolStripMenuItem TsiDefault = new ToolStripMenuItem(AppString.Menu.SetDefault);
        readonly ToolStripMenuItem TsiSetTop = new ToolStripMenuItem(AppString.Menu.SetTop);
        readonly ToolStripMenuItem TsiSetBottom = new ToolStripMenuItem(AppString.Menu.SetBottom);
        readonly ToolStripMenuItem TsiOnlyWithShift = new ToolStripMenuItem(AppString.Menu.OnlyWithShift);
        readonly ToolStripMenuItem TsiOnlyInExplorer = new ToolStripMenuItem(AppString.Menu.OnlyInExplorer);
        readonly ToolStripMenuItem TsiNoWorkDir = new ToolStripMenuItem(AppString.Menu.NoWorkingDirectory);
        readonly ToolStripMenuItem TsiNeverDefault = new ToolStripMenuItem(AppString.Menu.NeverDefault);
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);
        readonly ToolStripMenuItem TsiShowAsDisabled = new ToolStripMenuItem(AppString.Menu.ShowAsDisabledIfHidden);

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
            TsiRegExport = new RegExportMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText, new ToolStripSeparator(), TsiItemIcon,
                TsiPosition, TsiOtherAttributes, new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe});

            TsiItemIcon.DropDownItems.AddRange(new ToolStripItem[] { TsiChangeIcon, TsiDeleteIcon, TsiShieldIcon });

            TsiPosition.DropDownItems.AddRange(new ToolStripItem[] { TsiDefault, TsiSetTop, TsiSetBottom });

            TsiOtherAttributes.DropDownItems.AddRange(new ToolStripItem[] { TsiOnlyWithShift, TsiOnlyInExplorer,
                TsiNoWorkDir, TsiNeverDefault, TsiShowAsDisabled });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiChangeCommand, TsiFileProperties, TsiFileLocation, TsiRegLocation, TsiRegExport});

            TsiDeleteIcon.Click += (sender, e) => DeleteIcon();
            TsiSetTop.Click += (sender, e) => this.ItemPosition = Positions.Top;
            TsiSetBottom.Click += (sender, e) => this.ItemPosition = Positions.Bottom;
            TsiDefault.Click += (sender, e) => this.ItemPosition = Positions.Default;
            TsiOnlyInExplorer.Click += (sender, e) => this.OnlyInExplorer = !TsiOnlyInExplorer.Checked;
            TsiOnlyWithShift.Click += (sender, e) => this.OnlyWithShift = !TsiOnlyWithShift.Checked;
            TsiNoWorkDir.Click += (sender, e) => this.NoWorkingDirectory = !TsiNoWorkDir.Checked;
            TsiNeverDefault.Click += (sender, e) => this.NeverDefault = !TsiNeverDefault.Checked;
            TsiShowAsDisabled.Click += (sender, e) => SetDisabled(!TsiShowAsDisabled.Checked);
            ContextMenuStrip.Opening += (sender, e) => RefreshMenuItem();
            BtnSubItems.MouseDown += (sender, e) => ShowSubItems();
            TsiShieldIcon.Click += (sender, e) => UseShieldIcon();
            MyToolTip.SetToolTip(BtnSubItems, AppString.Tip.EditSubItems);
            this.AddCtr(BtnSubItems);
        }

        private void DeleteIcon()
        {
            this.IconLocation = null;
            this.HasLUAShield = false;
            this.Image = this.Image.ToTransparent();
        }

        private void UseShieldIcon()
        {
            bool flag = this.HasLUAShield = TsiShieldIcon.Checked = !TsiShieldIcon.Checked;
            if(IconLocation == null)
            {
                if(flag)
                {
                    this.Image = AppImage.Shield;
                    this.IconPath = "imageres.dll";
                    this.IconIndex = -78;
                }
                else
                {
                    this.Image = this.Image.ToTransparent();
                }
            }
        }

        private void SetDisabled(bool flag)
        {
            this.ShowAsDisabledIfHidden = flag;
            if(!ItemVisible && flag) ItemVisible = false;
        }

        private void RefreshMenuItem()
        {
            TsiOnlyWithShift.Visible = !IsSubItem;
            TsiDeleteMe.Enabled = !(IsOpenItem && AppConfig.ProtectOpenItem);
            TsiNoWorkDir.Checked = this.NoWorkingDirectory;
            TsiShowAsDisabled.Visible = WindowsOsVersion.IsAfterOrEqualWin10_1703;
            TsiShowAsDisabled.Checked = this.ShowAsDisabledIfHidden;
            TsiChangeCommand.Visible = !IsMultiItem && Guid.Equals(Guid.Empty);
            if(!this.IsSubItem) TsiOnlyWithShift.Checked = this.OnlyWithShift;

            if(WindowsOsVersion.IsAfterVista)
            {
                TsiItemIcon.Visible = true;
                TsiPosition.Visible = !IsSubItem;
                TsiOnlyInExplorer.Visible = !IsSubItem;
                TsiNeverDefault.Visible = !IsSubItem;
                if(this.HasIcon)
                {
                    TsiChangeIcon.Text = AppString.Menu.ChangeIcon;
                    TsiDeleteIcon.Visible = true;
                }
                else
                {
                    TsiChangeIcon.Text = AppString.Menu.AddIcon;
                    TsiDeleteIcon.Visible = false;
                }
                TsiShieldIcon.Checked = HasLUAShield;

                if(!IsSubItem)
                {
                    TsiOnlyInExplorer.Checked = this.OnlyInExplorer;
                    TsiNeverDefault.Checked = this.NeverDefault;
                    TsiDefault.Checked = TsiSetTop.Checked = TsiSetBottom.Checked = false;
                    switch(this.ItemPosition)
                    {
                        case Positions.Default:
                            TsiDefault.Checked = true;
                            break;
                        case Positions.Top:
                            TsiSetTop.Checked = true;
                            break;
                        case Positions.Bottom:
                            TsiSetBottom.Checked = true;
                            break;
                    }
                }
            }
            else
            {
                TsiItemIcon.Visible = false;
                TsiPosition.Visible = false;
                TsiOnlyInExplorer.Visible = false;
                TsiNeverDefault.Visible = false;
            }
        }

        private void ShowSubItems()
        {
            if(WindowsOsVersion.IsEqualVista)
            {
                MessageBoxEx.Show(AppString.Message.VistaUnsupportedMulti);
                return;
            }
            using(ShellSubMenuDialog dlg = new ShellSubMenuDialog())
            {
                dlg.Text = AppString.Dialog.EditSubItems.Replace("%s", this.Text);
                dlg.Icon = ResourceIcon.GetIcon(IconPath, IconIndex);
                dlg.ShowDialog(this.RegPath);
            }
        }

        private bool TryProtectOpenItem()
        {
            if(!IsOpenItem) return false;
            if(!AppConfig.ProtectOpenItem) return false;
            return MessageBoxEx.Show(AppString.Message.PromptIsOpenItem, MessageBoxButtons.YesNo) != DialogResult.Yes;
        }

        public virtual void DeleteMe()
        {
            try
            {
                RegistryEx.DeleteKeyTree(this.RegPath, true);
            }
            catch
            {
                MessageBoxEx.Show(AppString.Message.AuthorityProtection);
                return;
            }
            this.Dispose();
        }
    }
}