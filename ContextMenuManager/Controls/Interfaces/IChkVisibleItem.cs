using BluePointLilac.Controls;
using ContextMenuManager.Methods;

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
            this.CheckChanged += () => item.ItemVisible = this.Checked;
            listItem.ParentChanged += (sender, e) =>
            {
                if(listItem.IsDisposed) return;
                if(listItem.Parent == null) return;
                this.Checked = item.ItemVisible;
                if(listItem is FoldSubItem subItem && subItem.FoldGroupItem != null) return;
                if(listItem.FindForm() is ShellStoreDialog.ShellStoreForm) return;
                if(AppConfig.HideDisabledItems) listItem.Visible = this.Checked;
            };
        }
    }
}