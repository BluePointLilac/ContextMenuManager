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
            ((MyListItem)item).AddCtr(this);
            this.MouseDown += (sender, e) =>
            {
                if(MessageBoxEx.Show(AppString.Message.ConfirmDelete,
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                    item.DeleteMe();
            };
        }
    }
}