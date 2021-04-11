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
        /// <param name="ctr">目标控件</param>
        public static void CanMoveForm(this Control ctr)
        {
            DateTime downTime = DateTime.MinValue;
            DateTime upTime = DateTime.MinValue;
            ctr.MouseDown += (sender, e) => downTime = DateTime.Now;
            ctr.MouseUp += (sender, e) => upTime = DateTime.Now;
            ctr.MouseMove += (sender, e) =>
            {
                foreach(DateTime time in new[] { downTime, upTime })
                {
                    //避免ReleaseCapture影响控件的其他鼠标事件
                    if((DateTime.Now - time).TotalMilliseconds < 20) return;
                }
                if(e.Button == MouseButtons.Left)
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
    }
}