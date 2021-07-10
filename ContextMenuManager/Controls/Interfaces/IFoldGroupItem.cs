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
                    if(ctr is IFoldSubItem item1 && item1.FoldGroupItem == FoldGroup) ctr.Visible = !value;
                    else if(ctr is SubGroupItem item2 && item2.FoldGroupItem == FoldGroup) { item2.IsFold = true; item2.Visible = !value; }
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
            set
            {
                if(BtnFold.IsFold == value) return;
                BtnFold.IsFold = value;
                //IsFoldChanegd?.Invoke();
            }
        }

        //public Action IsFoldChanegd;
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
            ToolTipBox.SetToolTip(BtnOpenPath, tip);
            this.ImageDoubleClick += () => this.OnDoubleClick(null);
            this.TextDoubleClick += () => this.OnDoubleClick(null);
            this.DoubleClick += (sender, e) => this.IsFold = !this.IsFold;
        }

        public void HideWhenNoSubItem()
        {
            int count = 0;
            foreach(Control ctr in this.Parent.Controls)
            {
                if(ctr is IFoldSubItem item1 && item1.FoldGroupItem == this) count++;
                else if(ctr is SubGroupItem item2 && item2.FoldGroupItem == this) count++;
            }
            if(count == 0) this.Visible = false;
        }
    }

    class SubGroupItem : GroupPathItem
    {
        public SubGroupItem(string targetPath, PathType pathType) : base(targetPath, pathType) { }
        public GroupPathItem FoldGroupItem { get; set; }
    }
}