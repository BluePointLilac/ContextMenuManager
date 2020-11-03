using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.SpecialItems
{
    //所有文件的右键菜单项目：共享到Skype
    //目前仅发现这一个特殊例子，不符合所有通用规则
    sealed class SkypeShareItem : MyListItem, IChkVisibleItem, ITsiFilePathItem, ITsiRegPathItem, ITsiWebSearchItem, IBtnShowMenuItem
    {
        const string DllPath = @"Skype\SkypeContext.dll";
        const string GuidName = "{776DBC8D-7347-478C-8D71-791E12EF49D8}";
        const string PackageRegPath = @"HKEY_CLASSES_ROOT\PackagedCom\Package";
        const string AppxRegPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Appx";

        public SkypeShareItem()
        {
            InitializeComponents();
            string path = ItemFilePath;
            if(File.Exists(path))
            {
                ChkVisible.Checked = this.ItemVisible;
                this.Text = ResourceString.GetDirectString($"@{path},-101");
                string exePath = $@"{Path.GetDirectoryName(path)}\Skype.exe";
                this.Image = Icon.ExtractAssociatedIcon(exePath)?.ToBitmap();
            }
            else this.Visible = false;
        }

        public string ItemFilePath
        {
            get
            {
                string AppxDirPath = Registry.GetValue(AppxRegPath, "PackageRoot", null)?.ToString();
                if(!Directory.Exists(AppxDirPath)) return null;
                foreach(DirectoryInfo di in new DirectoryInfo(AppxDirPath).GetDirectories())
                {
                    if(di.Name.StartsWith("Microsoft.SkypeApp", StringComparison.OrdinalIgnoreCase))
                    {
                        string value = $@"{di.FullName}\{DllPath}";
                        if(File.Exists(value)) return value;
                    }
                }
                return null;
            }
        }

        private string PackageName
        {
            get
            {
                if(!File.Exists(ItemFilePath)) return null;
                string[] strs = ItemFilePath.Split('\\');
                return strs[strs.Length - 3];
            }
        }

        public string RegPath
        {
            get
            {
                if(PackageName == null) return null;
                return $@"{PackageRegPath}\{PackageName}\Class\{GuidName}";
            }
            set { }
        }

        public bool ItemVisible
        {
            get
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(RegPath))
                {
                    if(key == null) return true;
                    string value = key.GetValue("DllPath")?.ToString();
                    return value.Equals(DllPath, StringComparison.OrdinalIgnoreCase);
                }
            }
            set
            {
                Registry.SetValue(RegPath, "DllPath", value ? DllPath : string.Empty);
            }
        }

        public string SearchText => Text;

        public MenuButton BtnShowMenu { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiSearch,
                new ToolStripSeparator(), TsiFileProperties, TsiFileLocation, TsiRegLocation });
        }
    }
}