using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System;

namespace ContextMenuManager.Controls
{
    class NewItem : MyListItem
    {
        public NewItem()
        {
            this.Image = AppImage.NewItem;
            this.Text = AppString.Item.NewItem;
            this.SetNoClickEvent();
            this.AddCtr(BtnAddNewItem);
            MyToolTip.SetToolTip(BtnAddNewItem, AppString.Item.NewItem);
            BtnAddNewItem.MouseDown += (sender, e) => AddNewItem?.Invoke(null, null);
        }
        public event EventHandler AddNewItem;
        readonly PictureButton BtnAddNewItem = new PictureButton(AppImage.AddNewItem);
    }
}