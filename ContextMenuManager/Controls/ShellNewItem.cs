using BulePointLilac.Controls;
using BulePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellNewItem : MyListItem, IChkVisibleItem, ITsiTextItem, IBtnShowMenuItem,
        ITsiIconItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiDeleteItem
    {
        public static readonly string[] SnParts = { "ShellNew", "-ShellNew" };

        public ShellNewItem(string regPath)
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
                ChkVisible.Checked = this.ItemVisible;
            }
        }

        public string SearchText => $"{AppString.SideBar_New} {Text}";
        private string Extension => RegPath.Split('\\')[1];
        private string SnKeyName => RegistryEx.GetKeyName(RegPath);
        private string BuckupPath => $@"{RegistryEx.GetParentPath(RegPath)}\{(ItemVisible ? SnParts[1] : SnParts[0])}";

        private const string HKCR = "HKEY_CLASSES_ROOT";
        private string TypePath => $@"{HKCR}\{FileExtensionDialog.GetTypeName(Extension)}";//关联类型路径
        private string DefaultTypePath => $@"{HKCR}\{FileExtensionDialog.GetTypeName(Extension, false)}";//默认关联类型路径
        private string TypeDefaultIcon => Registry.GetValue($@"{TypePath}\DefaultIcon", "", null)?.ToString();//关联类型默认图标路径
        private string DefaultTypeDefaultIcon => Registry.GetValue($@"{DefaultTypePath}\DefaultIcon", "", null)?.ToString();//默认关联类型默认图标路径
        private bool IsFolderItem => Registry.GetValue(RegPath, "Directory", null) != null;

        private bool CanEditData
        {
            get
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(RegPath))
                {
                    foreach(string valueName in new[] { "Directory", "FileName", "Handler" })
                        if(key.GetValue(valueName) != null) return false;
                    if(key.GetValue("Data") != null && key.GetValueKind("Data") != RegistryValueKind.String) return false;
                    else return true;
                }
            }
        }

        public string ItemFilePath
        {
            get
            {
                string filePath = null;
                using(RegistryKey key = RegistryEx.GetRegistryKey(DefaultTypePath))
                {
                    if(key == null) return filePath;
                    string value = key.OpenSubKey(@"shell\open\command")?.GetValue("")?.ToString();
                    filePath = ObjectPath.ExtractFilePath(value);
                    if(filePath != null) return filePath;

                    value = key.OpenSubKey("CLSID")?.GetValue("")?.ToString();
                    if(Guid.TryParse(value, out Guid guid))
                    {
                        filePath = GuidInfo.GetFilePath(guid);
                        if(filePath != null) return filePath;
                    }
                }
                return filePath;
            }
        }

        public bool ItemVisible
        {
            get => SnKeyName.Equals(SnParts[0], StringComparison.OrdinalIgnoreCase);
            set
            {
                using(RegistryKey srcKey = RegistryEx.GetRegistryKey(RegPath))
                using(RegistryKey dstkey = RegistryEx.GetRegistryKey(BuckupPath, true, true))
                    srcKey.CopyTo(dstkey);
                RegistryEx.DeleteKeyTree(RegPath);
                this.RegPath = BuckupPath;
            }
        }

        public string ItemText
        {
            get
            {
                string name = Registry.GetValue(RegPath, "MenuText", null)?.ToString();
                name = ResourceString.GetDirectString(name);
                if(!string.IsNullOrEmpty(name)) return name;
                name = Registry.GetValue(DefaultTypePath, "FriendlyTypeName", null)?.ToString();
                name = ResourceString.GetDirectString(name);
                if(!string.IsNullOrEmpty(name)) return name;

                name = Registry.GetValue(DefaultTypePath, "", null)?.ToString();
                if(!string.IsNullOrEmpty(name)) return name;
                return null;
            }
            set
            {
                RegistryEx.DeleteValue(RegPath, "MenuText");
                Registry.SetValue(DefaultTypePath, "FriendlyTypeName", value);
            }
        }

        public string IconLocation
        {
            get => Registry.GetValue(RegPath, "IconPath", null)?.ToString();
            set => Registry.SetValue(RegPath, "IconPath", value);
        }

        public Icon ItemIcon
        {
            get
            {
                if(TypeDefaultIcon != null && TypeDefaultIcon.StartsWith("@"))
                    return ResourceIcon.GetExtensionIcon(Extension);

                Icon icon;
                string iconPath;
                int iconIndex;
                string value = IconLocation;
                if(string.IsNullOrWhiteSpace(value)) value = DefaultTypeDefaultIcon;
                if(!string.IsNullOrEmpty(value)) icon = ResourceIcon.GetIcon(value, out iconPath, out iconIndex);
                else icon = ResourceIcon.GetIcon(iconPath = ItemFilePath, iconIndex = 0);
                if(icon == null) icon = ResourceIcon.GetIcon(iconPath = "imageres.dll", iconIndex = 2);//图标资源不存在
                IconPath = iconPath; IconIndex = iconIndex;
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

        public VisibleCheckBox ChkVisible { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        public ChangeIconMenuItem TsiChangeIcon { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu_Details);
        readonly ToolStripMenuItem TsiEditData = new ToolStripMenuItem(AppString.Menu_InitialData);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiChangeIcon = new ChangeIconMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] {TsiChangeText,
                new ToolStripSeparator(), TsiChangeIcon, new ToolStripSeparator(),
                TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiEditData, TsiFileProperties, TsiFileLocation, TsiRegLocation });

            TsiEditData.Click += (sender, e) => EditInitialData();
            ContextMenuStrip.Opening += (sender, e) =>
            {
                TsiEditData.Visible = CanEditData;
                TsiDeleteMe.Enabled = !IsFolderItem;
            };
        }

        private void EditInitialData()
        {
            if(MessageBoxEx.Show(AppString.MessageBox_EditInitialData,
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            using(InputDialog dlg = new InputDialog
            {
                Title = AppString.Menu_InitialData,
                Text = this.InitialData?.ToString()
            })
            {
                if(dlg.ShowDialog() == DialogResult.OK) this.InitialData = dlg.Text;
            }
        }

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            RegistryEx.DeleteKeyTree(this.BuckupPath);
            this.Dispose();
        }
    }
}