using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class WinXGroupItem : GroupPathItem, IChkVisibleItem, ITsiDeleteItem, ITsiTextItem
    {
        public WinXGroupItem(string groupPath) : base(groupPath, ObjectPath.PathType.Directory)
        {
            InitializeComponents();
            this.TargetPath = groupPath;
        }

        public new string TargetPath
        {
            get => base.TargetPath;
            set
            {
                base.TargetPath = value;
                this.Text = Path.GetFileNameWithoutExtension(value);
                this.Image = ResourceIcon.GetFolderIcon(value).ToBitmap();
                ChkVisible.Checked = this.ItemVisible;
            }
        }

        public bool ItemVisible
        {
            get => (File.GetAttributes(TargetPath) & FileAttributes.Hidden) != FileAttributes.Hidden;
            set
            {
                FileAttributes attributes = File.GetAttributes(TargetPath);
                if(value) attributes &= ~FileAttributes.Hidden;
                else attributes |= FileAttributes.Hidden;
                File.SetAttributes(TargetPath, attributes);
                if(Directory.GetFiles(TargetPath).Length > 0) ExplorerRestarter.Show();
            }
        }

        public string ItemText
        {
            get => Path.GetFileNameWithoutExtension(TargetPath);
            set
            {
                string newPath = $@"{WinXList.WinXPath}\{ObjectPath.RemoveIllegalChars(value)}";
                Directory.Move(TargetPath, newPath);
                this.TargetPath = newPath;
                ExplorerRestarter.Show();
            }
        }

        public VisibleCheckBox ChkVisible { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        public ChangeTextMenuItem TsiChangeText { get; set; }
        readonly ToolStripMenuItem TsiRestoreDefault = new ToolStripMenuItem(AppString.Menu.RestoreDefault);

        private string DefaultGroupPath => $@"{WinXList.DefaultWinXPath}\{ItemText}";

        private void InitializeComponents()
        {
            ChkVisible = new VisibleCheckBox(this);
            this.SetCtrIndex(ChkVisible, 1);
            TsiDeleteMe = new DeleteMeMenuItem(this);
            TsiChangeText = new ChangeTextMenuItem(this);
            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiChangeText,
                new ToolStripSeparator(), TsiRestoreDefault, new ToolStripSeparator(), TsiDeleteMe });
            this.ContextMenuStrip.Opening += (sender, e) => TsiRestoreDefault.Enabled = Directory.Exists(DefaultGroupPath);
            TsiRestoreDefault.Click += (sender, e) => RestoreDefault();
        }

        private void RestoreDefault()
        {
            if(MessageBoxEx.Show(AppString.MessageBox.RestoreDefault, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                File.SetAttributes(TargetPath, File.GetAttributes(DefaultGroupPath));
                string[] paths = Directory.GetFiles(TargetPath);
                foreach(string path in paths)
                {
                    File.Delete(path);
                }
                paths = Directory.GetFiles(DefaultGroupPath);
                foreach(string path in paths)
                {
                    File.Copy(path, $@"{TargetPath}\{Path.GetFileName(path)}");
                }
                WinXList list = (WinXList)this.Parent;
                list.ClearItems();
                list.LoadItems();
                ExplorerRestarter.Show();
            }
        }

        public void DeleteMe()
        {
            bool flag = Directory.GetFiles(TargetPath, "*.lnk").Length > 0;
            if(flag && MessageBoxEx.Show(AppString.MessageBox.DeleteGroup, MessageBoxButtons.OKCancel) != DialogResult.OK) return;
            File.SetAttributes(TargetPath, FileAttributes.Normal);
            Directory.Delete(TargetPath, true);
            if(flag)
            {
                WinXList list = (WinXList)this.Parent;
                list.ClearItems();
                list.LoadItems();
                ExplorerRestarter.Show();
            }
            else this.Dispose();
        }
    }
}