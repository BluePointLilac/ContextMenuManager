using BulePointLilac.Controls;
using System;

namespace ContextMenuManager.Controls
{
    class NewItem : MyListItem
    {
        public NewItem()
        {
            this.Image = AppImage.NewItem;
            this.Text = AppString.Text_NewItem;
            this.AddCtr(BtnAddNewItem);
            MyToolTip.SetToolTip(BtnAddNewItem, AppString.Text_NewItem);
            BtnAddNewItem.MouseDown += (sender, e) => NewItemAdd?.Invoke(null, null);
        }
        public event EventHandler NewItemAdd;
        readonly PictureButton BtnAddNewItem = new PictureButton(AppImage.AddNewItem);
    }
}