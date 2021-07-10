using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class WinXItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, IBtnMoveUpDownItem, ITsiAdministratorItem,
        ITsiTextItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiDeleteItem, IFoldSubItem, ITsiShortcutCommandItem
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
                this.ShellLink = new ShellLink(value);
                this.Text = this.ItemText;
                this.Image = this.ItemIcon.ToBitmap();
            }
        }

        public string ItemText
        {
            get
            {
                string name = ShellLink.Description?.Trim();
                if(name.IsNullOrWhiteSpace()) name = DesktopIni.GetLocalizedFileNames(FilePath, true);
                if(name == string.Empty) name = Path.GetFileNameWithoutExtension(FilePath);
                return name;
            }
            set
            {
                ShellLink.Description = value;
                ShellLink.Save();
                this.Text = ResourceString.GetDirectString(value);
                ExplorerRestarter.Show();
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
                ExplorerRestarter.Show();
            }
        }

        public Icon ItemIcon
        {
            get
            {
                ShellLink.ICONLOCATION iconLocation = ShellLink.IconLocation;
                string iconPath = iconLocation.IconPath;
                int iconIndex = iconLocation.IconIndex;
                if(string.IsNullOrEmpty(iconPath)) iconPath = FilePath;
                Icon icon = ResourceIcon.GetIcon(iconPath, iconIndex);
                if(icon == null)
                {
                    string path = ItemFilePath;
                    if(File.Exists(path)) icon = ResourceIcon.GetExtensionIcon(path);
                    else if(Directory.Exists(path)) icon = ResourceIcon.GetFolderIcon(path);
                }
                return icon;
            }
        }

        public string ItemFilePath
        {
            get
            {
                string path = ShellLink.TargetPath;
                if(!File.Exists(path) && !Directory.Exists(path)) path = FilePath;
                return path;
            }
        }

        public ShellLink ShellLink { get; private set; }
        public string SearchText => $"{AppString.SideBar.WinX} {Text}";
        private string FileName => Path.GetFileName(FilePath);

        public IFoldGroupItem FoldGroupItem { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public ShortcutCommandMenuItem TsiChangeCommand { get; set; }
        public RunAsAdministratorItem TsiAdministrator { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        public MoveButton BtnMoveUp { get; set; }
        public MoveButton BtnMoveDown { get; set; }

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);
        readonly ToolStripMenuItem TsiChangeGroup = new ToolStripMenuItem(AppString.Menu.ChangeGroup);
        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            BtnMoveDown = new MoveButton(this, false);
            BtnMoveUp = new MoveButton(this, true);
            TsiChangeText = new ChangeTextMenuItem(this);
            TsiChangeCommand = new ShortcutCommandMenuItem(this);
            TsiAdministrator = new RunAsAdministratorItem(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText, new ToolStripSeparator(),
                TsiChangeGroup, new ToolStripSeparator(), TsiAdministrator, new ToolStripSeparator(),
                TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch,
                new ToolStripSeparator(), TsiChangeCommand, TsiFileProperties, TsiFileLocation });

            TsiChangeGroup.Click += (sender, e) => ChangeGroup();
            BtnMoveDown.MouseDown += (sender, e) => MoveItem(false);
            BtnMoveUp.MouseDown += (sender, e) => MoveItem(true);
            TsiChangeCommand.Click += (sender, e) =>
            {
                if(TsiChangeCommand.ChangeCommand(ShellLink))
                {
                    Image = ItemIcon.ToBitmap();
                    WinXHasher.HashLnk(FilePath);
                    ExplorerRestarter.Show();
                }
            };
        }

        private void ChangeGroup()
        {
            using(SelectDialog dlg = new SelectDialog())
            {
                dlg.Title = AppString.Dialog.SelectGroup;
                dlg.Items = WinXList.GetGroupNames();
                dlg.Selected = this.FoldGroupItem.Text;
                if(dlg.ShowDialog() != DialogResult.OK) return;
                if(dlg.Selected == this.FoldGroupItem.Text) return;
                string dirPath = $@"{WinXList.WinXPath}\{dlg.Selected}";
                int count = Directory.GetFiles(dirPath, "*.lnk").Length;
                string num = (count + 1).ToString().PadLeft(2, '0');
                string partName = this.FileName;
                int index = partName.IndexOf(" - ");
                if(index > 0) partName = partName.Substring(index + 3);
                string lnkPath = $@"{dirPath}\{num} - {partName}";
                lnkPath = ObjectPath.GetNewPathWithIndex(lnkPath, ObjectPath.PathType.File);
                string text = DesktopIni.GetLocalizedFileNames(FilePath);
                DesktopIni.DeleteLocalizedFileNames(FilePath);
                if(text != string.Empty) DesktopIni.SetLocalizedFileNames(lnkPath, text);
                File.Move(FilePath, lnkPath);
                this.FilePath = lnkPath;
                WinXList list = (WinXList)this.Parent;
                list.Controls.Remove(this);
                for(int i = 0; i < list.Controls.Count; i++)
                {
                    if(list.Controls[i] is WinXGroupItem groupItem && groupItem.Text == dlg.Selected)
                    {
                        list.Controls.Add(this);
                        list.SetItemIndex(this, i + 1);
                        this.Visible = !groupItem.IsFold;
                        this.FoldGroupItem = groupItem;
                        break;
                    }
                }
                ExplorerRestarter.Show();
            }
        }

        private void MoveItem(bool isUp)
        {
            WinXList list = (WinXList)this.Parent;
            int index = list.Controls.GetChildIndex(this);
            if(index == list.Controls.Count - 1) return;
            index += isUp ? -1 : 1;
            Control ctr = list.Controls[index];
            if(ctr is WinXGroupItem) return;
            WinXItem item = (WinXItem)ctr;
            string name1 = DesktopIni.GetLocalizedFileNames(this.FilePath);
            string name2 = DesktopIni.GetLocalizedFileNames(item.FilePath);
            DesktopIni.DeleteLocalizedFileNames(this.FilePath);
            DesktopIni.DeleteLocalizedFileNames(item.FilePath);
            string fileName1 = $@"{item.FileName.Substring(0, 2)}{this.FileName.Substring(2)}";
            string fileName2 = $@"{this.FileName.Substring(0, 2)}{item.FileName.Substring(2)}";
            string dirPath = Path.GetDirectoryName(this.FilePath);
            string path1 = $@"{dirPath}\{fileName1}";
            string path2 = $@"{dirPath}\{fileName2}";
            path1 = ObjectPath.GetNewPathWithIndex(path1, ObjectPath.PathType.File);
            path2 = ObjectPath.GetNewPathWithIndex(path2, ObjectPath.PathType.File);
            File.Move(this.FilePath, path1);
            File.Move(item.FilePath, path2);
            if(name1 != string.Empty) DesktopIni.SetLocalizedFileNames(path1, name1);
            if(name1 != string.Empty) DesktopIni.SetLocalizedFileNames(path2, name2);
            this.FilePath = path1;
            item.FilePath = path2;
            list.SetItemIndex(this, index);
            ExplorerRestarter.Show();
        }

        public void DeleteMe()
        {
            File.Delete(FilePath);
            DesktopIni.DeleteLocalizedFileNames(FilePath);
            ExplorerRestarter.Show();
            this.ShellLink.Dispose();
            this.Dispose();
        }
    }
}