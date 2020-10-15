using BulePointLilac.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    interface ITsiDeleteItem
    {
        DeleteMeMenuItem TsiDeleteMe { get; set; }
        void DeleteMe();
    }

    sealed class DeleteMeMenuItem : ToolStripMenuItem
    {
        public DeleteMeMenuItem(ITsiDeleteItem item) : base(AppString.Menu_Delete)
        {
            this.Click += (sender, e) =>
            {
                if(MessageBoxEx.Show(AppString.MessageBox_ConfirmDeletePermanently,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    item.DeleteMe();
            };
        }
    }
}