using BulePointLilac.Controls;
using static BulePointLilac.Methods.ObjectPath;

namespace ContextMenuManager.Controls
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
            this.MouseDown += (sender, e) => ShowPath(item.TargetPath, item.PathType);
        }
    }
}