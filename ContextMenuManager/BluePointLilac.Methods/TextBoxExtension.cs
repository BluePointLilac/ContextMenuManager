using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Methods
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
                box.Font = new Font(box.Font.FontFamily, size + (e.Delta > 0 ? 1F : -1F));
            };
        }

        /// <summary>TextBox在文字未超出边界时隐藏滚动条，超出时显示</summary>
        public static void SetAutoShowScroll(this TextBox box, ScrollBars scrollBars)
        {

            void SetScrollVisible()
            {
                Size szBox = box.ClientSize;
                Size szText = TextRenderer.MeasureText(box.Text, box.Font);
                if((scrollBars | ScrollBars.Vertical) == ScrollBars.Vertical)
                {
                    if(szText.Height > szBox.Height)
                    {
                        box.ScrollBars = scrollBars | ScrollBars.Vertical;
                    }
                    else
                    {
                        box.ScrollBars = scrollBars & ~ScrollBars.Vertical;
                    }
                }
                if((scrollBars | ScrollBars.Horizontal) == ScrollBars.Horizontal)
                {
                    if(szText.Width > szBox.Width)
                    {
                        box.ScrollBars = scrollBars | ScrollBars.Horizontal;
                    }
                    else
                    {
                        box.ScrollBars = scrollBars & ~ScrollBars.Horizontal;
                    }
                }
            };
            box.TextChanged += (sender, e) => SetScrollVisible();
            box.FontChanged += (sender, e) => SetScrollVisible();
            box.ClientSizeChanged += (sender, e) => SetScrollVisible();
        }

        /// <summary>TextBox只读时可以使用Ctrl+A全选快捷键</summary>
        public static void CanSelectAllWhenReadOnly(this TextBox box)
        {
            box.KeyDown += (sender, e) =>
            {
                if(box.ReadOnly && e.Control && e.KeyCode == Keys.A) box.SelectAll();
            };
        }
    }
}