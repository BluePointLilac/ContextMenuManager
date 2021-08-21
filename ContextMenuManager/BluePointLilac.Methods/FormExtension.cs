using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    public static class FormExtension
    {
        /// <summary>移动窗体时同时移动另一个窗体</summary>
        public static void MoveAsMove(this Form frm1, Form frm2)
        {
            if(frm2 == null) return;
            Point pLast = Point.Empty;
            frm1.Load += (sender, e) => pLast = frm1.Location;
            frm1.LocationChanged += (sender, e) =>
            {
                if(pLast == Point.Empty) return;
                frm2.Left += frm1.Left - pLast.X;
                frm2.Top += frm1.Top - pLast.Y;
                pLast = frm1.Location;
            };
        }

        /// <summary>给窗体添加ESC键关闭功能</summary>
        /// <remarks>也可以重写Form的ProcessDialogKey事件，
        /// 这个方法更简单，但遍历窗体控件时切记多了一个不可见的关闭按钮</remarks>
        public static void AddEscapeButton(this Form frm, DialogResult dr = DialogResult.Cancel)
        {
            Button btn = new Button
            {
                Parent = frm,
                Size = Size.Empty,
                DialogResult = dr
            };
            frm.CancelButton = btn;
            frm.Disposed += (sender, e) => btn.Dispose();
            frm.FormClosing += (sender, e) => btn.PerformClick();
        }
    }
}