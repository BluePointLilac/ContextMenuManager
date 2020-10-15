using System;
using System.Drawing;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    public class PictureButton : PictureBox
    {
        public PictureButton(Image image)
        {
            this.BaseImage = image;
            this.SizeMode = PictureBoxSizeMode.AutoSize;
            this.Cursor = Cursors.Hand;
        }

        private Image baseImage;
        public Image BaseImage
        {
            get => baseImage;
            set
            {
                baseImage = value;
                this.Image = ToolStripRenderer.CreateDisabledImage(value);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e); this.Image = BaseImage;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Image = ToolStripRenderer.CreateDisabledImage(BaseImage);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left) base.OnMouseDown(e);
        }
    }
}