using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BluePointLilac.Methods
{
    public static class ImageExtension
    {
        public static Image ToTransparent(this Image image, float opacity = 0.5F)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            using(Graphics g = Graphics.FromImage(bitmap))
            using(ImageAttributes attributes = new ImageAttributes())
            {
                ColorMatrix matrix = new ColorMatrix { Matrix33 = opacity };
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bitmap;
        }

        public static Image ResizeImage(this Image image, int width, int height)
        {
            //return image.GetThumbnailImage(width, height, null, System.IntPtr.Zero);//质量稍微低一点
            if(image.Width == width && image.Height == height) return image;
            Bitmap destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using(Graphics g = Graphics.FromImage(destImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;

                using(ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(image, new Rectangle(0, 0, width, height),
                        0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return destImage;
        }

        public static Image ResizeImage(this Image image, double scale)
        {
            if(scale == 1) return image;
            int width = (int)(image.Width * scale);
            int height = (int)(image.Height * scale);
            return image.ResizeImage(width, height);
        }

        public static Image ResizeImage(this Image image, Size newSize)
        {
            if(newSize == image.Size) return image;
            return image.ResizeImage(newSize.Width, newSize.Height);
        }

        public static Image RotateImage(this Image image, RotateFlipType rotateType)
        {
            Bitmap bitmap = new Bitmap(image);
            bitmap.RotateFlip(rotateType);
            return bitmap;
        }
    }
}