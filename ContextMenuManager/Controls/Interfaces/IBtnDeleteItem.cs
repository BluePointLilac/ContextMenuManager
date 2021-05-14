using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface IBtnDeleteItem
    {
        DeleteButton BtnDelete { get; set; }
        void DeleteMe();
    }

    sealed class DeleteButton : PictureButton
    {
        public DeleteButton(IBtnDeleteItem item) : base(AppImage.Delete)
        {
            MyListItem listItem = (MyListItem)item;
            listItem.AddCtr(this);
            this.MouseDown += (sender, e) =>
            {
                if(MessageBoxEx.Show(AppString.Message.ConfirmDelete,
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MyList list = (MyList)listItem.Parent;
                    int index = list.GetItemIndex(listItem);
                    item.DeleteMe();
                    list.HoveredItem = (MyListItem)list.Controls[index - 1];
                }
            };
        }
    }
}