using BluePointLilac.Controls;
using ContextMenuManager.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface IBtnShowMenuItem
    {
        ContextMenuStrip ContextMenuStrip { get; set; }
        MenuButton BtnShowMenu { get; set; }
    }

    sealed class MenuButton : PictureButton
    {
        public MenuButton(IBtnShowMenuItem item) : base(AppImage.Setting)
        {
            item.ContextMenuStrip = new ContextMenuStrip();
            ((MyListItem)item).AddCtr(this);
            bool isShow = false;
            this.MouseDown += (sender, e) =>
            {
                if(!isShow) item.ContextMenuStrip.Show(this, 0, Height);
                else item.ContextMenuStrip.Close();
                isShow = !isShow;
            };
        }
    }
}