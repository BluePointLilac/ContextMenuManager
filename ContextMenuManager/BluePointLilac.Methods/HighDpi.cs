using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    /// <summary>处理不同DPI缩放比下的像素绘制和字体显示问题</summary>
    /// <remarks>使用此类需要添加引用 PresentationFramework
    /// 还应在配置清单App.manifest中启用DPI感知自动缩放
    /// Font为矢量类型，Point、Size、Rectangle、Padding等为像素类型。
    /// 在不同DPI缩放下，矢量类型等比缩放，像素类型保持不变，故会出现排版显示问题。
    /// 解决方案一：项目中所有用到的像素类型实例值都取与缩放比之积，矢量类型不变。
    /// 解决方案二：项目中所有用到的矢量类型实例都取与缩放比之商，像素类型不变</remarks>
    public static class HighDpi
    {
        /// <summary>DPI缩放比</summary>
        public static readonly double DpiScale = Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;

        public static Point DpiZoom(this Point point) => new Point(DpiZoom(point.X), DpiZoom(point.Y));

        public static PointF DpiZoom(this PointF point) => new PointF(DpiZoom(point.X), DpiZoom(point.Y));

        public static Size DpiZoom(this Size size) => new Size(DpiZoom(size.Width), DpiZoom(size.Height));

        public static SizeF DpiZoom(this SizeF size) => new SizeF(DpiZoom(size.Width), DpiZoom(size.Height));

        public static Rectangle DpiZoom(this Rectangle r) => new Rectangle(DpiZoom(r.Location), DpiZoom(r.Size));

        public static RectangleF DpiZoom(this RectangleF r) => new RectangleF(DpiZoom(r.Location), DpiZoom(r.Size));

        public static Padding DpiZoom(this Padding p) => new Padding(DpiZoom(p.Left), DpiZoom(p.Top), DpiZoom(p.Right), DpiZoom(p.Bottom));

        public static Font DpiZoom(this Font font) => new Font(font.FontFamily, font.Size / DpiZoom(1F), font.Style);

        public static int DpiZoom(this int num) => (int)(num * DpiScale);

        public static float DpiZoom(this float num) => (float)(num * DpiScale);

        public static double DpiZoom(this double num) => num * DpiScale;
    }
}