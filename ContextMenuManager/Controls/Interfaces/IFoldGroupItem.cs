using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static BluePointLilac.Methods.ObjectPath;

namespace ContextMenuManager.Controls.Interfaces
{
    interface IFoldGroupItem
    {
        FoldButton BtnFold { get; set; }
        bool IsFold { get; set; }
        string Text { get; set; }
    }

    interface IFoldSubItem
    {
        IFoldGroupItem FoldGroupItem { get; set; }
    }

    sealed class FoldButton : PictureButton
    {
        private bool isFold;
        public bool IsFold
        {
            get => isFold;
            set
            {
                isFold = value;
                this.BaseImage = ReplaceImage(value);
                Control list = ((MyListItem)FoldGroup).Parent;
                if(list == null) return;
                list.SuspendLayout();
                foreach(Control ctr in list.Controls)
                {
                    if(ctr is IFoldSubItem item && item.FoldGroupItem == FoldGroup) ctr.Visible = !value;
                }
                list.ResumeLayout();
            }
        }

        private IFoldGroupItem FoldGroup { get; set; }

        static Image ReplaceImage(bool fold) => fold ? AppImage.Up : AppImage.Down;

        public FoldButton(IFoldGroupItem owner, bool fold = false) : base(ReplaceImage(fold))
        {
            this.FoldGroup = owner;
            ((MyListItem)owner).AddCtr(this);
            this.MouseDown += (sender, e) =>
            {
                this.IsFold = !this.IsFold;
                this.Image = this.BaseImage;
            };
        }
    }

    class GroupPathItem : MyListItem, IFoldGroupItem, IBtnOpenPathItem
    {
        public bool IsFold
        {
            get => BtnFold.IsFold;
            set => BtnFold.IsFold = value;
        }
        public string TargetPath { get; set; }
        public PathType PathType { get; set; }
        public ObjectPathButton BtnOpenPath { get; set; }
        public FoldButton BtnFold { get; set; }

        public GroupPathItem(string targetPath, PathType pathType)
        {
            BtnFold = new FoldButton(this);
            BtnOpenPath = new ObjectPathButton(this);
            this.Font = new Font(base.Font, FontStyle.Bold);
            if(pathType == PathType.File || pathType == PathType.Directory)
            {
                targetPath = Environment.ExpandEnvironmentVariables(targetPath);
            }
            this.TargetPath = targetPath;
            this.PathType = pathType;
            string tip = null;
            switch(pathType)
            {
                case PathType.File:
                    tip = AppString.Menu.FileLocation;
                    Text = Path.GetFileNameWithoutExtension(targetPath);
                    Image = ResourceIcon.GetExtensionIcon(targetPath).ToBitmap();
                    break;
                case PathType.Directory:
                    tip = AppString.Menu.FileLocation;
                    Text = Path.GetFileNameWithoutExtension(targetPath);
                    Image = ResourceIcon.GetFolderIcon(targetPath).ToBitmap();
                    break;
                case PathType.Registry:
                    tip = AppString.Menu.RegistryLocation;
                    break;
            }
            MyToolTip.SetToolTip(BtnOpenPath, tip);
            this.ImageDoubleClick += (sender, e) => this.OnDoubleClick(null);
            this.TextDoubleClick += (sender, e) => this.OnDoubleClick(null);
        }

        public void HideWhenNoSubItem()
        {
            int count = 0;
            foreach(var ctr in this.Parent.Controls)
            {
                if(ctr is IFoldSubItem item && item.FoldGroupItem == this) count++;
            }
            if(count == 0) this.Visible = false;
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            IsFold = !IsFold;
        }
    }
}