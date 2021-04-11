using BluePointLilac.Controls;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
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
            MyListItem listItem = (MyListItem)item;
            listItem.AddCtr(this);
            this.MouseDown += (sender, e) =>
            {
                if(e.Button == MouseButtons.Left)
                {
                    item.ItemVisible = !this.Checked;
                    this.Checked = item.ItemVisible;
                }
            };
            listItem.ParentChanged += (sender, e) =>
            {
                if(listItem.IsDisposed) return;
                if(listItem.Parent == null) return;
                if(listItem is IFoldSubItem subItem && subItem.FoldGroupItem != null) return;
                if(listItem.FindForm() is ShellStoreDialog.ShellStoreForm) return;
                if(AppConfig.HideDisabledItems) listItem.Visible = item.ItemVisible;
            };
        }
    }
}