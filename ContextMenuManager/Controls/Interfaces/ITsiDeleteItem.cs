using BulePointLilac.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiDeleteItem
    {
        DeleteMeMenuItem TsiDeleteMe { get; set; }
        void DeleteMe();
    }

    sealed class DeleteMeMenuItem : ToolStripMenuItem
    {
        public DeleteMeMenuItem(ITsiDeleteItem item) : base(AppString.Menu.Delete)
        {
            this.Click += (sender, e) =>
            {
                if(MessageBoxEx.Show(AppString.MessageBox.ConfirmDeletePermanently,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    item.DeleteMe();
            };
        }
    }
}