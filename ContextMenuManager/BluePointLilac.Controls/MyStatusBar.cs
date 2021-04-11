using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    public sealed class MyStatusBar : Panel
    {
        public static readonly string DefaultText = $"Ver: {Application.ProductVersion}    {Application.CompanyName}";

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
            set
            {
                if(base.Text == value) return;
                base.Text = value; Refresh();
            }
        }
        public new Font Font
        {
            get => base.Font;
            set
            {
                if(base.Font == value) return;
                base.Font = value; Refresh();
            }
        }
        public new Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                if(base.ForeColor == value) return;
                base.ForeColor = value; Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            string txt = this.Text;
            int left = this.Height / 3;
            for(int i = Text.Length - 1; i >= 0; i--)
            {
                Size size = TextRenderer.MeasureText(txt, Font);
                if(size.Width < ClientSize.Width - 2 * left)
                {
                    int top = (this.Height - size.Height) / 2;
                    e.Graphics.Clear(BackColor);
                    e.Graphics.DrawString(txt, Font, new SolidBrush(ForeColor), left, top);
                    break;
                }
                txt = Text.Substring(0, i) + "...";
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Refresh();
        }
    }
}