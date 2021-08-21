using ContextMenuManager.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class EnhanceMenusDialog : CommonDialog
    {
        public string ScenePath { get; set; }

        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(SubItemsForm frm = new SubItemsForm())
            using(EnhanceMenusList list = new EnhanceMenusList())
            {
                frm.Text = AppString.SideBar.EnhanceMenu;
                frm.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                frm.TopMost = AppConfig.TopMost;
                frm.AddList(list);
                list.ScenePath = this.ScenePath;
                list.UseUserDic = XmlDicHelper.EnhanceMenuPathDic[this.ScenePath];
                list.LoadItems();
                frm.ShowDialog();
            }
            return false;
        }
    }
}