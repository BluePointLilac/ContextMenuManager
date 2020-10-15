using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    public sealed class IconDialog : CommonDialog
    {
        const int MAXLENGTH = 260;//文件名最大长度
        private int iconIndex;
        public int IconIndex { get => iconIndex; set => iconIndex = value; }
        public string IconPath { get; set; }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            StringBuilder sb = new StringBuilder(IconPath, MAXLENGTH);
            bool flag = PickIconDlg(hwndOwner, sb, MAXLENGTH, ref iconIndex);
            IconPath = flag ? sb.ToString() : null;
            return flag;
        }

        public override void Reset() { }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, EntryPoint = "#62", SetLastError = true)]
        private static extern bool PickIconDlg(IntPtr hWnd, StringBuilder pszFileName, int cchFileNameMax, ref int pnIconIndex);
    }
}