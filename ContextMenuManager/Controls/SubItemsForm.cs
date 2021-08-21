using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class SubItemsForm : Form
    {
        public SubItemsForm()
        {
            this.SuspendLayout();
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = this.MaximizeBox = this.MinimizeBox = false;
            this.MinimumSize = this.Size = new Size(646, 419).DpiZoom();
            this.Controls.AddRange(new Control[] { listBox, statusBar });
            statusBar.CanMoveForm();
            this.AddEscapeButton();
            this.ResumeLayout();
        }

        readonly MyListBox listBox = new MyListBox { Dock = DockStyle.Fill };
        readonly MyStatusBar statusBar = new MyStatusBar();

        public void AddList(MyList myList)
        {
            myList.Owner = listBox;
            myList.HoveredItemChanged += (sender, e) =>
            {
                if(!AppConfig.ShowFilePath) return;
                MyListItem item = myList.HoveredItem;
                foreach(string prop in new[] { "ItemFilePath", "RegPath", "GroupPath" })
                {
                    string path = item.GetType().GetProperty(prop)?.GetValue(item, null)?.ToString();
                    if(!path.IsNullOrWhiteSpace()) { statusBar.Text = path; return; }
                }
                statusBar.Text = item.Text;
            };
        }
    }
}