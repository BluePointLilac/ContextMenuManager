using BluePointLilac.Controls;
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
            this.AddCtr(BtnAddNewItem);
            MyToolTip.SetToolTip(BtnAddNewItem, text);
            BtnAddNewItem.MouseDown += (sender, e) => AddNewItem?.Invoke(null, null);
            this.ImageDoubleClick += (sender, e) => AddNewItem?.Invoke(null, null);
            this.TextDoubleClick += (sender, e) => AddNewItem?.Invoke(null, null);

        }
        public event EventHandler AddNewItem;
        readonly PictureButton BtnAddNewItem = new PictureButton(AppImage.AddNewItem);
    }
}