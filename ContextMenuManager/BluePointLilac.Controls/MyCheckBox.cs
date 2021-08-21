using BluePointLilac.Methods;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    [DefaultProperty("Checked")]
    public class MyCheckBox : PictureBox
    {
        public MyCheckBox()
        {
            this.Image = TurnOff;
            this.Cursor = Cursors.Hand;
            this.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        private bool? _Checked = null;
        public bool Checked
        {
            get => _Checked == true;
            set
            {
                if(_Checked == value) return;
                this.Image = SwitchImage(value);
                if(_Checked == null)
                {
                    _Checked = value;
                    return;
                }
                if(PreCheckChanging != null && !PreCheckChanging.Invoke())
                {
                    this.Image = SwitchImage(!value);
                    return;
                }
                else CheckChanging?.Invoke();
                if(PreCheckChanged != null && !PreCheckChanged.Invoke())
                {
                    this.Image = SwitchImage(!value);
                    return;
                }
                else
                {
                    _Checked = value;
                    CheckChanged?.Invoke();
                }
            }
        }

        public Func<bool> PreCheckChanging;
        public Func<bool> PreCheckChanged;
        public Action CheckChanging;
        public Action CheckChanged;

        public Image TurnOnImage { get; set; } = TurnOn;
        public Image TurnOffImage { get; set; } = TurnOff;

        private Image SwitchImage(bool value)
        {
            return value ? TurnOnImage : TurnOffImage;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(e.Button == MouseButtons.Left) this.Checked = !this.Checked;
        }

        private static readonly Image TurnOn = DrawImage(true);
        private static readonly Image TurnOff = DrawImage(false);

        private static Image DrawImage(bool value)
        {
            int w = 80.DpiZoom();
            int r1 = 16.DpiZoom();
            float r2 = 13F.DpiZoom();
            int d1 = r1 * 2;
            float d2 = r2 * 2;
            float a = r1 - r2;
            Bitmap bitmap = new Bitmap(w, d1);
            using(Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                using(GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(new RectangleF(0, 0, d1, d1), 90, 180);
                    path.AddLine(new PointF(r1, 0), new PointF(w - r1, 0));
                    path.AddArc(new RectangleF(w - d1, 0, d1, d1), -90, 180);
                    path.AddLine(new PointF(w - r1, d1), new PointF(r1, d1));
                    Color color = value ? Color.FromArgb(0, 138, 217) : Color.FromArgb(130, 136, 144);
                    using(Brush brush = new SolidBrush(color))
                    {
                        g.FillPath(brush, path);
                    }
                }
                using(GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(new RectangleF(a, a, d2, d2), 90, 180);
                    path.AddLine(new PointF(r1, a), new PointF(w - r1, a));
                    path.AddArc(new RectangleF(w - d2 - a, a, d2, d2), -90, 180);
                    path.AddLine(new PointF(w - r1, d2 + a), new PointF(r1, d2 + a));
                    Color color = value ? Color.FromArgb(0, 162, 255) : Color.FromArgb(153, 160, 169);
                    using(Brush brush = new SolidBrush(color))
                    {
                        g.FillPath(brush, path);
                    }
                }
                using(GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(new RectangleF(value ? (w - d2 - a) : a, a, d2, d2));
                    g.FillPath(Brushes.White, path);
                }
            }
            return bitmap;
        }
    }
}