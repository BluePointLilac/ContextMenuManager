using ContextMenuManager.Controls.Interfaces;
using ContextMenuManager.Methods;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class WinXGroupItem : FoldGroupItem, IChkVisibleItem, ITsiDeleteItem, ITsiTextItem
    {
        public WinXGroupItem(string groupPath) : base(groupPath, ObjectPath.PathType.Directory)
        {
            InitializeComponents();
        }

        public bool ItemVisible
        {
            get => (File.GetAttributes(GroupPath) & FileAttributes.Hidden) != FileAttributes.Hidden;
            set
            {
                FileAttributes attributes = File.GetAttributes(GroupPath);
                if(value) attributes &= ~FileAttributes.Hidden;
                else attributes |= FileAttributes.Hidden;
                File.SetAttributes(GroupPath, attributes);
                if(Directory.GetFiles(GroupPath).Length > 0) ExplorerRestarter.Show();
            }
        }

        public string ItemText
        {
            get => Path.GetFileNameWithoutExtension(GroupPath);
            set
            {
                string newPath = $@"{WinXList.WinXPath}\{ObjectPath.RemoveIllegalChars(value)}";
                Directory.Move(GroupPath, newPath);
                this.GroupPath = newPath;
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
            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { new ToolStripSeparator(),
                TsiChangeText, TsiRestoreDefault, new ToolStripSeparator(), TsiDeleteMe });
            this.ContextMenuStrip.Opening += (sender, e) => TsiRestoreDefault.Enabled = Directory.Exists(DefaultGroupPath);
            TsiRestoreDefault.Click += (sender, e) => RestoreDefault();
        }

        private void RestoreDefault()
        {
            if(AppMessageBox.Show(AppString.Message.RestoreDefault, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                File.SetAttributes(GroupPath, FileAttributes.Normal);
                Directory.Delete(GroupPath, true);
                Directory.CreateDirectory(GroupPath);
                File.SetAttributes(GroupPath, File.GetAttributes(DefaultGroupPath));
                foreach(string srcPath in Directory.GetFiles(DefaultGroupPath))
                {
                    string dstPath = $@"{GroupPath}\{Path.GetFileName(srcPath)}";
                    File.Copy(srcPath, dstPath);
                }
                WinXList list = (WinXList)this.Parent;
                list.ClearItems();
                list.LoadItems();
                ExplorerRestarter.Show();
            }
        }

        public void DeleteMe()
        {
            bool flag = Directory.GetFiles(GroupPath, "*.lnk").Length > 0;
            if(flag && AppMessageBox.Show(AppString.Message.DeleteGroup, MessageBoxButtons.OKCancel) != DialogResult.OK) return;
            File.SetAttributes(GroupPath, FileAttributes.Normal);
            Directory.Delete(GroupPath, true);
            if(flag)
            {
                WinXList list = (WinXList)this.Parent;
                list.ClearItems();
                list.LoadItems();
                ExplorerRestarter.Show();
            }
        }
    }
}