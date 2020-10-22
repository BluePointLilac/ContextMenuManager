using BulePointLilac.Methods;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class SubMenuModeForm : Form
    {
        public SubMenuModeForm()
        {
            this.Text = AppString.General.AppName;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ControlBox = this.ShowIcon = this.ShowInTaskbar = false;
            this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 9F);
            this.Controls.AddRange(new Control[] { pnlTop, btnOne, btnTwo });
            pnlTop.Controls.Add(lblInfo);
            int a = 20.DpiZoom();
            this.ClientSize = new Size(lblInfo.Width + 2 * a, lblInfo.Height + btnOne.Height + 3 * a);
            lblInfo.Location = new Point(a, a);
            pnlTop.Height = lblInfo.Bottom + a;
            btnOne.Top = btnTwo.Top = pnlTop.Bottom + a / 2;
            btnTwo.Left = pnlTop.Width - btnTwo.Width - a;
            btnOne.Left = btnTwo.Left - btnOne.Width - a;
            btnOne.Click += (sender, e) => SubMenuMode = 1;
            btnTwo.Click += (sender, e) => SubMenuMode = 2;
        }

        public int SubMenuMode { get; private set; }

        readonly Label lblInfo = new Label
        {
            AutoSize = true,
            Text = AppString.Text.SelectSubMenuMode,
        };

        readonly Panel pnlTop = new Panel
        {
            BackColor = Color.White,
            Dock = DockStyle.Top
        };

        readonly Button btnOne = new Button
        {
            DialogResult = DialogResult.OK,
            AutoSize = true,
            Text = "①",
        };

        readonly Button btnTwo = new Button
        {
            DialogResult = DialogResult.OK,
            AutoSize = true,
            Text = "②",
        };
    }
}