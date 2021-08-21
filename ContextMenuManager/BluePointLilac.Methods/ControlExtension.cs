using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    public static class ControlExtension
    {
        [DllImport("user32.dll")]
        private static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        /// <summary>使控件能够移动所属窗体</summary>
        /// <param name="ctr">目标控件</param>
        public static void CanMoveForm(this Control ctr)
        {
            const int WM_NCLBUTTONDOWN = 0xA1;
            const int HT_CAPTION = 0x2;
            ctr.MouseMove += (sender, e) =>
            {
                if(e.Button != MouseButtons.Left) return;
                ReleaseCapture();
                PostMessage(ctr.FindForm().Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            };
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int wndproc);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>通过Win32API禁用/启用目标控件</summary>
        /// <remarks>控件被禁用时仍可更改字体颜色，不需要同时设置ctr.Enabled=false</remarks>
        /// <param name="ctr">目标控件</param>
        /// <param name="enabled">启用为true，禁用为false</param>
        public static void SetEnabled(this Control ctr, bool enabled)
        {
            const int GWL_STYLE = -16;
            const int WS_DISABLED = 0x8000000;
            int value = GetWindowLong(ctr.Handle, GWL_STYLE);
            if(enabled) value &= ~WS_DISABLED;
            else value |= WS_DISABLED;
            SetWindowLong(ctr.Handle, GWL_STYLE, value);
        }
    }
}