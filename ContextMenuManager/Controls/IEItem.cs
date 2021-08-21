using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using ContextMenuManager.Methods;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class IEItem : MyListItem, ITsiRegPathItem, ITsiFilePathItem, ITsiRegDeleteItem, ITsiCommandItem,
        ITsiWebSearchItem, ITsiTextItem, ITsiRegExportItem, IBtnShowMenuItem, IChkVisibleItem
    {
        public static readonly string[] MeParts = { "MenuExt", "-MenuExt" };

        public IEItem(string regPath)
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
                this.Image = this.ItemImage;
            }
        }
        public string ValueName => null;
        private string KeyName => RegistryEx.GetKeyName(RegPath);
        private string BackupPath => $@"{IEList.IEPath}\{(ItemVisible ? MeParts[1] : MeParts[0])}\{KeyName}";
        private string MeKeyName => RegistryEx.GetKeyName(RegistryEx.GetParentPath(RegPath));

        public string ItemText
        {
            get => RegistryEx.GetKeyName(RegPath);
            set
            {
                string newPath = $@"{RegistryEx.GetParentPath(RegPath)}\{value.Replace("\\", "")}";
                string defaultValue = Registry.GetValue(newPath, "", null)?.ToString();
                if(!defaultValue.IsNullOrWhiteSpace())
                {
                    AppMessageBox.Show(AppString.Message.HasBeenAdded);
                }
                else
                {
                    RegistryEx.MoveTo(RegPath, newPath);
                    RegPath = newPath;
                }
            }
        }

        public bool ItemVisible
        {
            get => MeKeyName.Equals(MeParts[0], StringComparison.OrdinalIgnoreCase);
            set
            {
                RegistryEx.MoveTo(RegPath, BackupPath);
                RegPath = BackupPath;
            }
        }

        public string ItemCommand
        {
            get => Registry.GetValue(RegPath, "", null)?.ToString();
            set
            {
                Registry.SetValue(RegPath, "", value);
                this.Image = this.ItemImage;
            }
        }

        public string SearchText => $@"{AppString.SideBar.IEMenu} {Text}";
        public string ItemFilePath => ObjectPath.ExtractFilePath(ItemCommand);
        private Icon ItemIcon => ResourceIcon.GetIcon(ItemFilePath) ?? ResourceIcon.GetExtensionIcon(ItemFilePath);
        private Image ItemImage => ItemIcon?.ToBitmap() ?? AppImage.NotFound;

        public MenuButton BtnShowMenu { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        public ChangeCommandMenuItem TsiChangeCommand { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public RegExportMenuItem TsiRegExport { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiChangeCommand = new ChangeCommandMenuItem(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiRegExport = new RegExportMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText,
                new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiChangeCommand, TsiFileProperties, TsiFileLocation, TsiRegLocation, TsiRegExport});
        }

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            RegistryEx.DeleteKeyTree(this.BackupPath);
        }
    }
}