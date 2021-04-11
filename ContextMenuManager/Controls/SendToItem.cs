using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class SendToItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiTextItem, ITsiAdministratorItem,
        ITsiIconItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiDeleteItem, ITsiShortcutCommandItem
    {
        public SendToItem(string filePath)
        {
            InitializeComponents();
            this.FilePath = filePath;
        }

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                if(IsShortcut) this.ShellLink = new ShellLink(value);
                this.Text = this.ItemText;
                this.Image = this.ItemIcon.ToBitmap();
                ChkVisible.Checked = this.ItemVisible;
            }
        }

        public ShellLink ShellLink { get; private set; }
        private string FileExtension => Path.GetExtension(FilePath);
        private bool IsShortcut => FileExtension.ToLower() == ".lnk";
        public string SearchText => $"{AppString.SideBar.SendTo} {Text}";

        public string ItemFilePath
        {
            get
            {
                string path = null;
                if(IsShortcut) path = ShellLink.TargetPath;
                else
                {
                    using(RegistryKey root = Registry.ClassesRoot)
                    using(RegistryKey extKey = root.OpenSubKey(FileExtension))
                    {
                        string guidPath = extKey?.GetValue("")?.ToString();
                        if(!string.IsNullOrEmpty(guidPath))
                        {
                            using(RegistryKey ipsKey = root.OpenSubKey($@"{guidPath}\InProcServer32"))
                            {
                                path = ipsKey?.GetValue("")?.ToString();
                            }
                        }
                    }
                }
                if(!File.Exists(path) && !Directory.Exists(path)) path = FilePath;
                return path;
            }
        }

        public bool ItemVisible
        {
            get => (File.GetAttributes(FilePath) & FileAttributes.Hidden) != FileAttributes.Hidden;
            set
            {
                FileAttributes attributes = File.GetAttributes(FilePath);
                if(value) attributes &= ~FileAttributes.Hidden;
                else attributes |= FileAttributes.Hidden;
                File.SetAttributes(FilePath, attributes);
            }
        }

        public string ItemText
        {
            get
            {
                string name = DesktopIni.GetLocalizedFileNames(FilePath, true);
                if(name == string.Empty) name = Path.GetFileNameWithoutExtension(FilePath);
                if(name == string.Empty) name = FileExtension;
                return name;
            }
            set
            {
                DesktopIni.SetLocalizedFileNames(FilePath, value);
                this.Text = ResourceString.GetDirectString(value);
                ExplorerRestarter.Show();
            }
        }

        public Icon ItemIcon
        {
            get
            {
                Icon icon = ResourceIcon.GetIcon(IconLocation, out string iconPath, out int iconIndex);
                IconPath = iconPath; IconIndex = iconIndex;
                if(icon != null) return icon;
                if(IsShortcut)
                {
                    string path = ItemFilePath;
                    if(File.Exists(path)) icon = ResourceIcon.GetExtensionIcon(path);
                    else if(Directory.Exists(path)) icon = ResourceIcon.GetFolderIcon(path);
                }
                if(icon == null) icon = ResourceIcon.GetExtensionIcon(FileExtension);
                return icon;
            }
        }

        public string IconLocation
        {
            get
            {
                string location = null;
                if(IsShortcut)
                {
                    ShellLink.ICONLOCATION iconLocation = ShellLink.IconLocation;
                    string iconPath = iconLocation.IconPath;
                    int iconIndex = iconLocation.IconIndex;
                    if(string.IsNullOrEmpty(iconPath)) iconPath = ShellLink.TargetPath;
                    location = $@"{iconPath},{iconIndex}";
                }
                else
                {
                    using(RegistryKey root = Registry.ClassesRoot)
                    using(RegistryKey extensionKey = root.OpenSubKey(FileExtension))
                    {
                        string guidPath = extensionKey.GetValue("")?.ToString();
                        if(guidPath != null)
                        {
                            using(RegistryKey guidKey = root.OpenSubKey($@"{guidPath}\DefaultIcon"))
                            {
                                location = guidKey.GetValue("")?.ToString();
                            }
                        }
                    }
                }
                return location;
            }
            set
            {
                if(IsShortcut)
                {
                    ShellLink.IconLocation = new ShellLink.ICONLOCATION
                    {
                        IconPath = this.IconPath,
                        IconIndex = this.IconIndex
                    };
                    ShellLink.Save();
                }
                else
                {
                    using(RegistryKey root = Registry.ClassesRoot)
                    using(RegistryKey extensionKey = root.OpenSubKey(FileExtension))
                    {
                        string guidPath = extensionKey.GetValue("")?.ToString();
                        if(guidPath != null)
                        {
                            string regPath = $@"{root.Name}\{guidPath}\DefaultIcon";
                            RegTrustedInstaller.TakeRegTreeOwnerShip(regPath);
                            Registry.SetValue(regPath, "", value);
                            ExplorerRestarter.Show();
                        }
                    }
                }
            }
        }

        public string IconPath { get; set; }
        public int IconIndex { get; set; }

        public VisibleCheckBox ChkVisible { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        public ChangeIconMenuItem TsiChangeIcon { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        public ShortcutCommandMenuItem TsiChangeCommand { get; set; }
        public RunAsAdministratorItem TsiAdministrator { get; set; }

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiChangeIcon = new ChangeIconMenuItem(this);
            TsiChangeCommand = new ShortcutCommandMenuItem(this);
            TsiAdministrator = new RunAsAdministratorItem(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText, new ToolStripSeparator(),
                TsiChangeIcon, new ToolStripSeparator(), TsiAdministrator, new ToolStripSeparator(),
                TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiChangeCommand, TsiFileProperties, TsiFileLocation });

            ContextMenuStrip.Opening += (sender, e) => TsiChangeCommand.Visible = IsShortcut;

            TsiChangeCommand.Click += (sender, e) =>
            {
                if(TsiChangeCommand.ChangeCommand(ShellLink))
                {
                    Image = ItemIcon.ToBitmap();
                }
            };
        }

        public void DeleteMe()
        {
            File.Delete(this.FilePath);
            DesktopIni.DeleteLocalizedFileNames(FilePath);
            //this.Shortcut.Dispose();
            this.Dispose();
        }
    }
}