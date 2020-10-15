using System.Drawing;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    public sealed class MyToolTip : Form
    {
        public static readonly MyToolTip Instance = new MyToolTip();

        public static void SetToolTip(Control ctr, string tip)
        {
            if(string.IsNullOrWhiteSpace(tip)) return;
            ctr.MouseEnter += (sender, e) =>
            {
                Instance.Text = tip;
                Instance.Size = Instance.LblTip.Size;
                Instance.Location = ctr.PointToScreen(new Point(ctr.Width + 10, (ctr.Height - Instance.Height) / 2));
                Instance.Visible = true;
                ctr.FindForm().Activate();
            };
            ctr.MouseLeave += (sender, e) => Instance.Visible = false;
        }

        static MyToolTip()
        {
            Instance.Opacity = 0;
            Instance.Show();
            Instance.Visible = false;
            Instance.Opacity = 1;
        }
        private MyToolTip()
        {
            this.TopMost = true;
            this.BackColor = Color.FromArgb(235, 245, 255);
            this.ForeColor = Color.FromArgb(80, 80, 80);
            this.ShowIcon = this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Controls.Add(LblTip);
        }
        public new string Text
        {
            get => LblTip.Text;
            set => LblTip.Text = value;
        }
        public new Font Font
        {
            get => LblTip.Font;
            set => LblTip.Font = value;
        }
        public new Color ForeColor
        {
            get => LblTip.ForeColor;
            set => LblTip.ForeColor = value;
        }
        public BorderStyle BorderStyle
        {
            get => LblTip.BorderStyle;
            set => LblTip.BorderStyle = value;
        }

        readonly Label LblTip = new Label
        {
            AutoSize = true,
            Font = SystemFonts.MenuFont,
            BorderStyle = BorderStyle.FixedSingle,
        };
    }
}