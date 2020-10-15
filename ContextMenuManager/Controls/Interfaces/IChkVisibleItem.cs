using BulePointLilac.Controls;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    interface IChkVisibleItem
    {
        bool ItemVisible { get; set; }
        VisibleCheckBox ChkVisible { get; set; }
    }

    sealed class VisibleCheckBox : MyCheckBox
    {
        public VisibleCheckBox(IChkVisibleItem item)
        {
            ((MyListItem)item).AddCtr(this);
            this.MouseDown += (sender, e) =>
            {
                if(e.Button == MouseButtons.Left)
                {
                    item.ItemVisible = !this.Checked;
                    this.Checked = item.ItemVisible;
                }
            };
        }
    }
}