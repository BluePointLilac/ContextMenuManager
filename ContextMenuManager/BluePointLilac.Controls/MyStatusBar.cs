using BluePointLilac.Methods;
using System;
using System.ComponentModel;
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

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text { get => base.Text; set => base.Text = value; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            string txt = this.Text;
            int left = this.Height / 3;
            for(int i = this.Text.Length - 1; i >= 0; i--)
            {
                Size size = TextRenderer.MeasureText(txt, this.Font);
                if(size.Width < ClientSize.Width - 2 * left)
                {
                    using(Brush brush = new SolidBrush(this.ForeColor))
                    {
                        int top = (this.Height - size.Height) / 2;
                        e.Graphics.Clear(this.BackColor);
                        e.Graphics.DrawString(txt, this.Font, brush, left, top);
                        break;
                    }
                }
                txt = this.Text.Substring(0, i) + "...";
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e); this.Refresh();
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e); this.Refresh();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e); this.Refresh();
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e); this.Refresh();
        }
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e); this.Refresh();
        }
    }
}