using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace mmhmmTest.Utils
{
    public static class ImageExtensions
    {
        public static BitmapSource ToBitmapSource(this Bitmap imgmap)
        {
            var imgmapData = imgmap.LockBits(new Rectangle(0, 0, imgmap.Width, imgmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, imgmap.PixelFormat);
            var imgmapSource = BitmapSource.Create(
                imgmapData.Width, imgmapData.Height, imgmap.HorizontalResolution, imgmap.VerticalResolution,
                PixelFormats.Bgr24, null, imgmapData.Scan0, imgmapData.Stride * imgmapData.Height, 
                imgmapData.Stride);

            imgmap.UnlockBits(imgmapData);
            return imgmapSource;
        }
    }
}
