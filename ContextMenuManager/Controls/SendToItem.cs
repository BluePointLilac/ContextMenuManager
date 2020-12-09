using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class SendToItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiTextItem,
        ITsiIconItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiDeleteItem
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
                if(IsShortcut) this.Shortcut.FullName = value;
                this.Text = this.ItemText;
                this.Image = this.ItemIcon.ToBitmap();
                ChkVisible.Checked = this.ItemVisible;
            }
        }

        private WshShortcut Shortcut = new WshShortcut();
        private string FileName => Path.GetFileName(FilePath);
        private string FileExtension => Path.GetExtension(FilePath);
        private bool IsShortcut => FileExtension.ToLower() == ".lnk";
        public string SearchText => $"{AppString.SideBar.SendTo} {Text}";

        public string ItemFilePath
        {
            get
            {
                if(IsShortcut) return Shortcut.TargetPath;
                else
                {
                    string guidPath = Registry.ClassesRoot.OpenSubKey(FileExtension)?.GetValue("")?.ToString();
                    if(string.IsNullOrEmpty(guidPath)) return null;
                    else return Registry.ClassesRoot.OpenSubKey($@"{guidPath}\InProcServer32")?.GetValue("")?.ToString();
                }
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
                string name = SendToList.DesktopIniReader.GetValue("LocalizedFileNames", FileName);
                name = ResourceString.GetDirectString(name);
                if(name == string.Empty) name = Path.GetFileNameWithoutExtension(FilePath);
                if(name == string.Empty) name = FileExtension;
                return name;
            }
            set
            {
                SendToList.DesktopIniWriter.SetValue("LocalizedFileNames", FileName, value);
                this.Text = ResourceString.GetDirectString(value);
                ExplorerRestarter.NeedRestart = true;
            }
        }

        public Icon ItemIcon
        {
            get
            {
                Icon icon = ResourceIcon.GetIcon(IconLocation, out string iconPath, out int iconIndex);
                IconPath = iconPath; IconIndex = iconIndex;
                if(icon == null && IsShortcut)
                {
                    if(File.Exists(Shortcut.TargetPath)) icon = Icon.ExtractAssociatedIcon(Shortcut.TargetPath);
                    else if(Directory.Exists(Shortcut.TargetPath)) icon = ResourceIcon.GetFolderIcon(Shortcut.TargetPath);
                }
                icon = icon ?? ResourceIcon.GetExtensionIcon(FileExtension);
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
                    location = Shortcut.IconLocation;
                    if(location.StartsWith(",")) location = $"{Shortcut.TargetPath}{location}";
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
                    Shortcut.IconLocation = value;
                    Shortcut.Save();
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
                            ExplorerRestarter.NeedRestart = true;
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
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);
        readonly ToolStripMenuItem TsiChangeCommand = new ToolStripMenuItem(AppString.Menu.ChangeCommand);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiChangeIcon = new ChangeIconMenuItem(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText, new ToolStripSeparator(),
                TsiChangeIcon, new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiChangeCommand, TsiFileProperties, TsiFileLocation });

            ContextMenuStrip.Opening += (sender, e) => TsiChangeCommand.Visible = IsShortcut;

            TsiChangeCommand.Click += (sender, e) => ChangeCommand();

        }

        private void ChangeCommand()
        {
            using(CommandDialog dlg = new CommandDialog())
            {
                dlg.Command = Shortcut.TargetPath;
                dlg.Arguments = Shortcut.Arguments;
                if(dlg.ShowDialog() != DialogResult.OK) return;
                Shortcut.TargetPath = dlg.Command;
                Shortcut.Arguments = dlg.Arguments;
                Shortcut.Save();
            }
        }

        public void DeleteMe()
        {
            File.Delete(this.FilePath);
            SendToList.DesktopIniWriter.DeleteKey("LocalizedFileNames", FileName);
            this.Dispose();
        }
    }
}