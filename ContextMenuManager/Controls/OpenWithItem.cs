using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using ContextMenuManager.Methods;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class OpenWithItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiTextItem,
        ITsiCommandItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiRegDeleteItem, ITsiRegExportItem
    {

        public OpenWithItem(string regPath)
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
                this.ItemFilePath = ObjectPath.ExtractFilePath(ItemCommand);
                this.Text = this.ItemText;
                this.Image = this.ItemIcon.ToBitmap();
            }
        }
        public string ValueName => null;
        private string ShellPath => RegistryEx.GetParentPath(RegPath);
        private string AppPath => RegistryEx.GetParentPath(RegistryEx.GetParentPath(ShellPath));
        private bool NameEquals => RegistryEx.GetKeyName(AppPath).Equals(Path.GetFileName(ItemFilePath), StringComparison.OrdinalIgnoreCase);
        private Icon ItemIcon => Icon.ExtractAssociatedIcon(ItemFilePath);

        public string ItemText
        {
            get
            {
                string name = null;
                if(NameEquals)
                {
                    name = Registry.GetValue(AppPath, "FriendlyAppName", null)?.ToString();
                    name = ResourceString.GetDirectString(name);
                }
                if(string.IsNullOrEmpty(name)) name = FileVersionInfo.GetVersionInfo(ItemFilePath).FileDescription;
                if(string.IsNullOrEmpty(name)) name = Path.GetFileName(ItemFilePath);
                return name;
            }
            set
            {
                Registry.SetValue(AppPath, "FriendlyAppName", value);
                this.Text = ResourceString.GetDirectString(value);
            }
        }

        public string ItemCommand
        {
            get => Registry.GetValue(RegPath, "", null)?.ToString();
            set
            {
                if(ObjectPath.ExtractFilePath(value) != ItemFilePath)
                {
                    AppMessageBox.Show(AppString.Message.CannotChangePath);
                }
                else Registry.SetValue(RegPath, "", value);
            }
        }

        public bool ItemVisible
        {
            get => Registry.GetValue(AppPath, "NoOpenWith", null) == null;
            set
            {
                if(value) RegistryEx.DeleteValue(AppPath, "NoOpenWith");
                else Registry.SetValue(AppPath, "NoOpenWith", "");
            }
        }

        public string SearchText => $"{AppString.SideBar.OpenWith} {Text}";
        public string ItemFilePath { get; private set; }

        public VisibleCheckBox ChkVisible { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        public ChangeCommandMenuItem TsiChangeCommand { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        public RegExportMenuItem TsiRegExport { get; set; }

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiChangeCommand = new ChangeCommandMenuItem(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiRegExport = new RegExportMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText,
                new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiChangeCommand, TsiFileProperties, TsiFileLocation, TsiRegLocation, TsiRegExport });

            ContextMenuStrip.Opening += (sender, e) => TsiChangeText.Enabled = this.NameEquals;
        }

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            using(RegistryKey key = RegistryEx.GetRegistryKey(ShellPath))
            {
                if(key.GetSubKeyNames().Length == 0) RegistryEx.DeleteKeyTree(this.AppPath);
            }
        }
    }
}