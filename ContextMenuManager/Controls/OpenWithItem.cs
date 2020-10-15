using BulePointLilac.Controls;
using BulePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class OpenWithItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiTextItem,
        ITsiCommandItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiDeleteItem
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
                ChkVisible.Checked = this.ItemVisible;
            }
        }

        private string AppPath => RegistryEx.GetParentPath(RegistryEx.GetParentPath(RegistryEx.GetParentPath(RegPath)));
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
            }
        }

        public string ItemCommand
        {
            get => Registry.GetValue(RegPath, "", null)?.ToString();
            set
            {
                if(ObjectPath.ExtractFilePath(value) != ItemFilePath)
                {
                    MessageBoxEx.Show(AppString.MessageBox_CannotChangePath);
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

        public string SearchText => $"{AppString.SideBar_OpenWith} {Text}";
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
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu_Details);

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
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText,
                new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiChangeCommand, TsiFileProperties, TsiFileLocation, TsiRegLocation });

            ContextMenuStrip.Opening += (sender, e) => TsiChangeText.Enabled = this.NameEquals;
        }

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            this.Dispose();
        }
    }
}