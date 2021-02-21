using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    public static class ControlExtension
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        /// <summary>使控件能够移动所属窗体</summary>
        /// <remarks>副作用：使用此方法将无法触发Click等事件</remarks>
        /// <param name="ctr">目标控件</param>
        public static void CanMoveForm(this Control ctr)
        {
            ctr.MouseDown += (sender, e) =>
            {
                if(e.Button == MouseButtons.Left && ctr.FindForm().WindowState == FormWindowState.Normal)
                {
                    ReleaseCapture();
                    SendMessage(ctr.FindForm().Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int wndproc);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        private const int GWL_STYLE = -16;
        private const int WS_DISABLED = 0x8000000;

        /// <summary>通过Win32API禁用/启用目标控件</summary>
        /// <remarks>控件被禁用时仍可更改字体颜色，不需要同时设置ctr.Enabled=false</remarks>
        /// <param name="ctr">目标控件</param>
        /// <param name="enabled">启用为true，禁用为false</param>
        public static void SetEnabled(this Control ctr, bool enabled)
        {
            if(enabled) { SetWindowLong(ctr.Handle, GWL_STYLE, (~WS_DISABLED) & GetWindowLong(ctr.Handle, GWL_STYLE)); }
            else { SetWindowLong(ctr.Handle, GWL_STYLE, WS_DISABLED | GetWindowLong(ctr.Handle, GWL_STYLE)); }
        }

        public static void SetNoClickEvent(this Control ctr)
        {
            Cursor cursor = ctr.Cursor;
            ctr.MouseDown += (sender, e) => ctr.Cursor = Cursors.No;
            ctr.MouseUp += (sender, e) => ctr.Cursor = cursor;
        }
    }
}