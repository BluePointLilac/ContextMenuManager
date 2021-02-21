using BluePointLilac.Controls;
using BluePointLilac.Methods;
using static BluePointLilac.Methods.ObjectPath;

namespace ContextMenuManager.Controls.Interfaces
{
    interface IBtnOpenPathItem
    {
        string TargetPath { get; set; }
        PathType PathType { get; set; }
        ObjectPathButton BtnOpenPath { get; set; }
    }

    sealed class ObjectPathButton : PictureButton
    {
        public ObjectPathButton(IBtnOpenPathItem item) : base(AppImage.Open)
        {
            ((MyListItem)item).AddCtr(this);
            this.MouseDown += (sender, e) =>
            {
                switch(item.PathType)
                {
                    case PathType.File:
                    case PathType.Directory:
                        ExternalProgram.JumpExplorer(item.TargetPath);
                        break;
                    case PathType.Registry:
                        ExternalProgram.JumpRegEdit(item.TargetPath, null, AppConfig.OpenMoreRegedit);
                        break;
                }
            };
        }
    }
}