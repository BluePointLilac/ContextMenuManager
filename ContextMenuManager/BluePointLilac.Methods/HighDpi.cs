using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    public static class HighDpi
    {
        public static readonly double DpiScale = Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;

        public static Point DpiZoom(this Point point) => new Point(DpiZoom(point.X), DpiZoom(point.Y));

        public static PointF DpiZoom(this PointF point) => new PointF(DpiZoom(point.X), DpiZoom(point.Y));

        public static Size DpiZoom(this Size size) => new Size(DpiZoom(size.Width), DpiZoom(size.Height));

        public static SizeF DpiZoom(this SizeF size) => new SizeF(DpiZoom(size.Width), DpiZoom(size.Height));

        public static Rectangle DpiZoom(this Rectangle r) => new Rectangle(DpiZoom(r.Location), DpiZoom(r.Size));

        public static RectangleF DpiZoom(this RectangleF r) => new RectangleF(DpiZoom(r.Location), DpiZoom(r.Size));

        public static Padding DpiZoom(this Padding p) => new Padding(DpiZoom(p.Left), DpiZoom(p.Top), DpiZoom(p.Right), DpiZoom(p.Bottom));

        public static int DpiZoom(this int num) => (int)(num * DpiScale);

        public static float DpiZoom(this float num) => (float)(num * DpiScale);

        public static double DpiZoom(this double num) => num * DpiScale;
    }
}