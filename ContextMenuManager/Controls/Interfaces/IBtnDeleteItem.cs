using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
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
                if(MessageBoxEx.Show(AppString.MessageBox.ConfirmDelete,
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                    item.DeleteMe();
            };
        }
    }
}