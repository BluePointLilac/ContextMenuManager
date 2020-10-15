using BulePointLilac.Controls;

namespace ContextMenuManager.Controls
{
    sealed class AddCommonButton : PictureButton
    {
        public AddCommonButton() : base(AppImage.AddCommon)
        {
            MyToolTip.SetToolTip(this, AppString.Tip_AddCommonItems);
        }
    }
}