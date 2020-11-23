using System.Drawing;
using System.Windows.Forms;

namespace BulePointLilac.Methods
{
    public static class TextBoxExtension
    {
        /// <summary>TextBox仿RichTextBox按住Ctrl加鼠标滚轮放缩字体</summary>
        public static void CanResizeFont(this TextBox box)
        {
            box.MouseWheel += (sender, e) =>
            {
                if(Control.ModifierKeys != Keys.Control) return;
                float size = box.Font.Size;
                if(size < 8F && e.Delta < 0) return;
                if(size > 40F && e.Delta > 0) return;
                box.Font = new Font(box.Font.FontFamily, size + (e.Delta > 0 ? 1 : -1));
            };
        }
    }
}