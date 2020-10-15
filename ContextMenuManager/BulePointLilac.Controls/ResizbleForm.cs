using System;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    /// <summary>限制水平、竖直方向调整大小的窗体</summary>
    class ResizbleForm : Form
    {
        /// <summary>水平方向可调整大小</summary>
        public bool HorizontalResizable { get; set; } = true;

        /// <summary>竖直方向可调整大小</summary>
        public bool VerticalResizable { get; set; } = true;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch(m.Msg)
            {
                case WM_NCHITTEST:
                    IntPtr hNowhere = new IntPtr((int)HitTest.Nowhere);
                    HitTest value = (HitTest)m.Result;
                    switch(value)
                    {
                        case HitTest.Top:
                        case HitTest.Bottom:
                            if(!VerticalResizable) m.Result = hNowhere;
                            break;
                        case HitTest.Left:
                        case HitTest.Right:
                            if(!HorizontalResizable) m.Result = hNowhere;
                            break;
                        case HitTest.TopLeft:
                        case HitTest.TopRight:
                        case HitTest.BottomLeft:
                        case HitTest.BottomRight:
                            if(!VerticalResizable || !HorizontalResizable) m.Result = hNowhere;
                            break;
                    }
                    break;
            }
        }

        /// <summary>光标移动或鼠标按下、释放时的消息</summary>
        private const int WM_NCHITTEST = 0x84;
        /// <summary>鼠标击中位置</summary>
        private enum HitTest : int
        {
            Error = -2,
            Transparent = -1,
            Nowhere = 0,
            Client = 1,
            TitleBar = 2,
            SysMenu = 3,
            Size = 4,
            GrowBox = 5,
            Hscroll = 6,
            Vscroll = 7,
            MinButton = 8,
            MaxButton = 9,
            Left = 10,
            Right = 11,
            Top = 12,
            TopLeft = 13,
            TopRight = 14,
            Bottom = 15,
            BottomLeft = 16,
            BottomRight = 17,
            Border = 18,
            Close = 20,
            Help = 21
        }
    }
}