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
            this.AddCtr(BtnAddNewItem);
            ToolTipBox.SetToolTip(BtnAddNewItem, text);
            BtnAddNewItem.MouseDown += (sender, e) => AddNewItem?.Invoke();
            this.ImageDoubleClick += () => AddNewItem?.Invoke();
            this.TextDoubleClick += () => AddNewItem?.Invoke();

        }
        public Action AddNewItem { get; set; }
        readonly PictureButton BtnAddNewItem = new PictureButton(AppImage.AddNewItem);
    }
}