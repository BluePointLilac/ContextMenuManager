using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class SelectItemsForm : Form
    {
        public SelectItemsForm()
        {
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
            this.Font = SystemFonts.MessageBoxFont;
            this.ShowIcon = this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = this.Size = new Size(652, 425).DpiZoom();
            list.Owner = listBox;
            InitializeComponents();
        }

        protected MyList list = new MyList();
        protected MyListBox listBox = new MyListBox();
        protected Panel pnlBorder = new Panel
        {
            BackColor = Color.FromArgb(200, 200, 200)
        };
        protected Button btnOk = new Button
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            DialogResult = DialogResult.OK,
            Text = AppString.Dialog.Ok,
            AutoSize = true
        };
        protected Button btnCancel = new Button
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            DialogResult = DialogResult.Cancel,
            Text = AppString.Dialog.Cancel,
            AutoSize = true
        };

        private void InitializeComponents()
        {
            this.Controls.AddRange(new Control[] { listBox, pnlBorder, btnOk, btnCancel });
            int a = 20.DpiZoom();
            listBox.Location = new Point(a, a);
            pnlBorder.Location = new Point(a - 1, a - 1);
            btnOk.Top = btnCancel.Top = this.ClientSize.Height - btnCancel.Height - a;
            btnCancel.Left = this.ClientSize.Width - btnCancel.Width - a;
            btnOk.Left = btnCancel.Left - btnOk.Width - a;
            this.OnResize(null);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            listBox.Width = ClientSize.Width - 2 * listBox.Left;
            listBox.Height = btnOk.Top - 2 * listBox.Top;
            pnlBorder.Width = listBox.Width + 2;
            pnlBorder.Height = listBox.Height + 2;
        }
    }
}