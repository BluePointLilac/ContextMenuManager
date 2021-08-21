using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using ContextMenuManager.Methods;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class UwpModeItem : MyListItem, IChkVisibleItem, ITsiRegPathItem, ITsiFilePathItem,
        IBtnShowMenuItem, ITsiWebSearchItem, ITsiRegExportItem, ITsiRegDeleteItem, ITsiGuidItem
    {
        public UwpModeItem(string uwpName, Guid guid)
        {
            this.Guid = guid;
            this.UwpName = uwpName;
            this.InitializeComponents();
            this.Visible = UwpHelper.GetPackageName(uwpName) != null;
            this.Image = GuidInfo.GetImage(guid);
            this.Text = this.ItemText;
        }

        public Guid Guid { get; set; }
        public string UwpName { get; set; }

        public bool ItemVisible
        {
            get
            {
                foreach(string path in GuidBlockedList.BlockedPaths)
                {
                    using(RegistryKey key = RegistryEx.GetRegistryKey(path))
                    {
                        if(key == null) continue;
                        if(key.GetValue(Guid.ToString("B")) != null) return false;
                    }
                }
                return true;
            }
            set
            {
                foreach(string path in GuidBlockedList.BlockedPaths)
                {
                    if(value)
                    {
                        RegistryEx.DeleteValue(path, Guid.ToString("B"));
                    }
                    else
                    {
                        Registry.SetValue(path, Guid.ToString("B"), "");
                    }
                }
                ExplorerRestarter.Show();
            }
        }

        public string ItemText => GuidInfo.GetText(Guid);
        public string RegPath => UwpHelper.GetRegPath(UwpName, Guid);
        public string ItemFilePath => UwpHelper.GetFilePath(UwpName, Guid);

        public string SearchText => Text;
        public string ValueName => "DllPath";
        public MenuButton BtnShowMenu { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public DetailedEditButton BtnDetailedEdit { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        public RegExportMenuItem TsiRegExport { get; set; }
        public HandleGuidMenuItem TsiHandleGuid { get; set; }

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            BtnDetailedEdit = new DetailedEditButton(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);
            TsiRegExport = new RegExportMenuItem(this);
            TsiHandleGuid = new HandleGuidMenuItem(this);

            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiHandleGuid,
                new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe });
            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiFileProperties, TsiFileLocation, TsiRegLocation, TsiRegExport });
        }

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
        }
    }
}