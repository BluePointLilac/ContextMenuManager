using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;

namespace ContextMenuManager.Controls
{
    class NewItem : MyListItem
    {
        public NewItem() : this(AppString.Other.NewItem) { }

        public NewItem(string text)
        {
            this.Text = text;
            this.Image = AppImage.NewItem;
            this.SetNoClickEvent();
            this.AddCtr(BtnAddNewItem);
            MyToolTip.SetToolTip(BtnAddNewItem, text);
            BtnAddNewItem.MouseDown += (sender, e) => AddNewItem?.Invoke(null, null);

        }
        public event EventHandler AddNewItem;
        readonly PictureButton BtnAddNewItem = new PictureButton(AppImage.AddNewItem);
    }
}