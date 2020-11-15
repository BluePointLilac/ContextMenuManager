using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class WinXItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem,
        ITsiTextItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiDeleteItem, IFoldSubItem
    {
        public WinXItem(string filePath, IFoldGroupItem group)
        {
            InitializeComponents();
            this.FoldGroupItem = group;
            this.FilePath = filePath;
        }

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                this.Shortcut.FullName = value;
                this.Text = this.ItemText;
                this.Image = this.ItemIcon.ToBitmap();
                ChkVisible.Checked = this.ItemVisible;
            }
        }

        public string ItemText
        {
            get
            {
                string name = Shortcut.Description.Trim();
                if(name == string.Empty) name = WinXList.GetMenuName(FilePath);
                if(name == string.Empty) name = Path.GetFileNameWithoutExtension(FilePath);
                return name;
            }
            set
            {
                Shortcut.Description = value;
                Shortcut.Save();
                ExplorerRestarter.NeedRestart = true;
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
                ExplorerRestarter.NeedRestart = true;
            }
        }

        private string IconLocation
        {
            get
            {
                if(Shortcut.IconLocation.StartsWith(","))
                    Shortcut.IconLocation = $"{Shortcut.TargetPath}{Shortcut.IconLocation}";
                return Shortcut.IconLocation;
            }
        }

        private WshShortcut Shortcut = new WshShortcut();
        private Icon ItemIcon => ResourceIcon.GetIcon(IconLocation) ?? Icon.ExtractAssociatedIcon(Shortcut.TargetPath);
        public string SearchText => $"{AppString.SideBar.WinX} {Text}";

        public string ItemFilePath
        {
            get
            {
                if(File.Exists(Shortcut.TargetPath)) return Shortcut.TargetPath;
                else return FilePath;
            }
        }

        public IFoldGroupItem FoldGroupItem { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText,
                new ToolStripSeparator(), TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch,
                new ToolStripSeparator(), TsiFileProperties, TsiFileLocation });
        }

        public void DeleteMe()
        {
            File.Delete(this.FilePath);
            this.Dispose();
        }
    }
}