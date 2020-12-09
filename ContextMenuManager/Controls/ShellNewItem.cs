using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellNewItem : MyListItem, IChkVisibleItem, ITsiTextItem, IBtnShowMenuItem, IBtnMoveUpDownItem,
         ITsiIconItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiRegDeleteItem, ITsiRegExportItem
    {
        public static readonly string[] SnParts = { "ShellNew", "-ShellNew" };
        public static readonly string[] UnableSortExtensions = { ".library-ms", ".lnk", "Folder" };
        private static readonly string[] UnableEditDataValues = { "Directory", "FileName", "Handler", "Command" };

        public ShellNewItem(ShellNewList list, string regPath)
        {
            this.Owner = list;
            InitializeComponents();
            this.RegPath = regPath;
            BtnMoveUp.Visible = BtnMoveDown.Visible = this.CanSort && LockNewItem.IsLocked();
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

        public string SearchText => $"{AppString.SideBar.New} {Text}";
        public string Extension => RegPath.Split('\\')[1];
        private string SnKeyName => RegistryEx.GetKeyName(RegPath);
        private string BackupPath => $@"{RegistryEx.GetParentPath(RegPath)}\{(ItemVisible ? SnParts[1] : SnParts[0])}";

        private const string HKCR = "HKEY_CLASSES_ROOT";
        private string TypePath => $@"{HKCR}\{FileExtensionDialog.GetTypeName(Extension)}";//关联类型路径
        private string DefaultTypePath => $@"{HKCR}\{FileExtensionDialog.GetTypeName(Extension, false)}";//默认关联类型路径
        private string TypeDefaultIcon => Registry.GetValue($@"{TypePath}\DefaultIcon", "", null)?.ToString();//关联类型默认图标路径
        private string DefaultTypeDefaultIcon => Registry.GetValue($@"{DefaultTypePath}\DefaultIcon", "", null)?.ToString();//默认关联类型默认图标路径
        private bool CanEditData => UnableEditDataValues.All(value => Registry.GetValue(RegPath, value, null) == null);//能够编辑初始数据的
        public bool CanSort => !UnableSortExtensions.Contains(Extension, StringComparer.OrdinalIgnoreCase);//能够排序的

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
                    if(GuidEx.TryParse(value, out Guid guid))
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
                RegistryEx.MoveTo(RegPath, BackupPath);
                this.RegPath = BackupPath;
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
                this.Text = ResourceString.GetDirectString(value);
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
                if(value.IsNullOrWhiteSpace()) value = DefaultTypeDefaultIcon;
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

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);
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
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiRegExport = new RegExportMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] {TsiChangeText,
                new ToolStripSeparator(), TsiChangeIcon, new ToolStripSeparator(),
                TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiEditData, TsiFileProperties, TsiFileLocation, TsiRegLocation, TsiRegExport });

            TsiEditData.Click += (sender, e) => EditInitialData();
            ContextMenuStrip.Opening += (sender, e) => TsiEditData.Visible = CanEditData;

            BtnMoveUp.MouseDown += (sender, e) => Owner.MoveItem(this, true);
            BtnMoveDown.MouseDown += (sender, e) => Owner.MoveItem(this, false);
        }

        private void EditInitialData()
        {
            if(MessageBoxEx.Show(AppString.MessageBox.EditInitialData,
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

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            RegistryEx.DeleteKeyTree(this.BackupPath);
            this.Dispose();
            if(LockNewItem.IsLocked()) Owner.WriteRegistry();
        }
    }
}