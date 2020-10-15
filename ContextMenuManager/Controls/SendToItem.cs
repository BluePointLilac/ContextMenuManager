using BulePointLilac.Controls;
using BulePointLilac.Methods;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class SendToItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, ITsiTextItem, ITsiIconItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiDeleteItem
    {

        private static readonly IWshRuntimeLibrary.WshShell WshShell = new IWshRuntimeLibrary.WshShell();

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
                if(IsShortcut) this.Shortcut = WshShell.CreateShortcut(value);
                this.Text = this.ItemText;
                this.Image = this.ItemIcon.ToBitmap();
                ChkVisible.Checked = this.ItemVisible;
            }
        }

        private IWshRuntimeLibrary.IWshShortcut Shortcut;
        private string FileName => Path.GetFileName(FilePath);
        private string FileExtension => Path.GetExtension(FilePath);
        private bool IsShortcut => FileExtension.ToLower() == ".lnk";
        public string SearchText => $"{AppString.SideBar_SendTo} {Text}";

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
                string name = SendToList.GetMenuName(FileName);
                if(name == string.Empty) name = Path.GetFileNameWithoutExtension(FilePath);
                if(name == string.Empty) name = FileExtension;
                return name;
            }
            set
            {
                DesktopIniHelper.SetLocalizedFileName(FilePath, value);
                ExplorerRestarter.NeedRestart = true;
            }
        }

        public Icon ItemIcon
        {
            get
            {
                Icon icon = null;
                if(IsShortcut)
                {
                    icon = ResourceIcon.GetIcon(IconLocation, out string iconPath, out int iconIndex);
                    IconPath = iconPath; IconIndex = iconIndex;
                    if(icon == null)
                    {
                        if(File.Exists(Shortcut.TargetPath)) icon = Icon.ExtractAssociatedIcon(Shortcut.TargetPath);
                        else if(Directory.Exists(Shortcut.TargetPath)) icon = ResourceIcon.GetFolderIcon(Shortcut.TargetPath);
                    }
                }
                icon = icon ?? ResourceIcon.GetExtensionIcon(FileExtension);
                return icon;
            }
        }

        public string IconLocation
        {
            get
            {
                string location = Shortcut.IconLocation;
                if(location.StartsWith(",")) location = $"{Shortcut.TargetPath}{location}";
                return location;
            }
            set
            {
                Shortcut.IconLocation = value;
                Shortcut.Save();
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
        readonly ToolStripSeparator TsiIconSeparator = new ToolStripSeparator();
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu_Details);

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

            ContextMenuStrip.Opening += (sender, e) => TsiChangeIcon.Visible = TsiIconSeparator.Visible = IsShortcut;

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText, new ToolStripSeparator(),
                TsiChangeIcon, TsiIconSeparator, TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiFileProperties, TsiFileLocation });
        }

        public void DeleteMe()
        {
            File.Delete(this.FilePath);
            DesktopIniHelper.DeleteLocalizedFileName(FilePath);
            this.Dispose();
        }
    }
}