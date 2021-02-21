using BluePointLilac.Controls;
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
            this.MouseDown += (sender, e) => item.ContextMenuStrip.Show(this, 0, Height);
        }
    }
}