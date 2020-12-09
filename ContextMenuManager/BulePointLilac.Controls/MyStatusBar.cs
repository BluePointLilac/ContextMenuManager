using BulePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    public class MyStatusBar : Panel
    {
        public static readonly string DefaultText = $"Ver: {new Version(Application.ProductVersion).ToString(2)}    {Application.CompanyName}";

        public MyStatusBar()
        {
            this.Text = DefaultText;
            this.Height = 30.DpiZoom();
            this.Dock = DockStyle.Bottom;
            this.Font = SystemFonts.StatusFont;
            this.BackColor = Color.FromArgb(70, 130, 200);
            this.ForeColor = Color.White;
        }

        public new string Text
        {
            get => base.Text;
            set { base.Text = value; Refresh(); }
        }
        public new Font Font
        {
            get => base.Font;
            set { base.Font = value; Refresh(); }
        }
        public new Color ForeColor
        {
            get => base.ForeColor;
            set { base.ForeColor = value; Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int left = this.Height / 3;
            int top = (this.Height - TextRenderer.MeasureText(this.Text, this.Font).Height) / 2;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), left, top);
        }
    }
}