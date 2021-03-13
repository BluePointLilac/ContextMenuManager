using System;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    /// <summary>限制水平、竖直方向调整大小的窗体</summary>
    public class ResizbleForm : Form
    {
        /// <summary>水平方向可调整大小</summary>
        public bool HorizontalResizable { get; set; } = true;

        /// <summary>竖直方向可调整大小</summary>
        public bool VerticalResizable { get; set; } = true;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if(m.Msg == HitTestMessage.WM_NCHITTEST && this.WindowState == FormWindowState.Normal)
            {
                IntPtr hNowhere = new IntPtr((int)HitTestMessage.HitTest.Nowhere);
                HitTestMessage.HitTest value = (HitTestMessage.HitTest)m.Result;
                switch(value)
                {
                    case HitTestMessage.HitTest.Top:
                    case HitTestMessage.HitTest.Bottom:
                        if(!VerticalResizable) m.Result = hNowhere;
                        break;
                    case HitTestMessage.HitTest.Left:
                    case HitTestMessage.HitTest.Right:
                        if(!HorizontalResizable) m.Result = hNowhere;
                        break;
                    case HitTestMessage.HitTest.TopLeft:
                    case HitTestMessage.HitTest.TopRight:
                    case HitTestMessage.HitTest.BottomLeft:
                    case HitTestMessage.HitTest.BottomRight:
                        if(!VerticalResizable || !HorizontalResizable) m.Result = hNowhere;
                        break;
                }
            }
        }
    }
}