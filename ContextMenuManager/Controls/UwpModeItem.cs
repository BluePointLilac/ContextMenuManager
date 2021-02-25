using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class UwpModeItem : MyListItem, IChkVisibleItem, ITsiRegPathItem, ITsiFilePathItem,
        IBtnShowMenuItem, ITsiWebSearchItem, ITsiRegExportItem, ITsiRegDeleteItem, ITsiGuidItem
    {
        private const string PackagePath = @"HKEY_CLASSES_ROOT\PackagedCom\Package";
        private const string AppXPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Appx";
        private static readonly string WindowAppsDir = Registry.GetValue(AppXPath, "PackageRoot", null)?.ToString();

        public UwpModeItem(string uwpName, Guid guid)
        {
            this.Guid = guid;
            this.UwpName = uwpName;
            this.InitializeComponents();
            ChkVisible.Checked = ItemVisible;
            this.Visible = File.Exists(ItemFilePath);
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
        public string RegPath => GetGuidRegPath(UwpName, Guid);
        public string ItemFilePath => GetFilePath(UwpName, Guid);

        public static string GetPackageName(string uwpName)
        {
            using(RegistryKey packageKey = RegistryEx.GetRegistryKey(PackagePath))
            {
                if(packageKey == null) return null;
                foreach(string packageName in packageKey.GetSubKeyNames())
                {
                    if(packageName.StartsWith(uwpName, StringComparison.OrdinalIgnoreCase))
                    {
                        return packageName;
                    }
                }
            }
            return null;
        }

        public static string GetGuidRegPath(string uwpName, Guid guid)
        {
            string packageName = GetPackageName(uwpName);
            if(packageName == null) return null;
            else return $@"{PackagePath}\{packageName}\Class\{guid:B}";
        }

        public static string GetFilePath(string uwpName, Guid guid)
        {
            string regPath = GetGuidRegPath(uwpName, guid);
            if(regPath == null) return null;
            string dirPath = $@"{WindowAppsDir}\{GetPackageName(uwpName)}";
            string path = Registry.GetValue(regPath, "DllPath", null)?.ToString();
            if(path.IsNullOrWhiteSpace()) return dirPath;
            else return $@"{dirPath}\{path}";
        }

        public string SearchText => Text;
        public string ValueName => "DllPath";
        public MenuButton BtnShowMenu { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
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
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);
            TsiRegExport = new RegExportMenuItem(this);
            TsiHandleGuid = new HandleGuidMenuItem(this, false);
            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiHandleGuid, 
                new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe });
            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiFileProperties, TsiFileLocation, TsiRegLocation, TsiRegExport });
        }

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            this.Dispose();
        }
    }
}