using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    sealed class ReadOnlyRichTextBox : RichTextBox
    {
        public ReadOnlyRichTextBox()
        {
            this.ReadOnly = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.None;
            this.ForeColor = Color.FromArgb(60, 60, 60);
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 10F);
        }

        const int WM_SETFOCUS = 0x0007;
        const int WM_KILLFOCUS = 0x0008;
        protected override void WndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WM_SETFOCUS:
                    m.Msg = WM_KILLFOCUS; break;
            }
            base.WndProc(ref m);
        }

        protected override void OnLinkClicked(LinkClickedEventArgs e)
        {
            base.OnLinkClicked(e); Process.Start(e.LinkText);
        }
    }
}