using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TimelineGenerator
{
    public class TimeLineRenderer
    {
        private readonly Border border;
        private readonly double scale = 2.5;

        public TimeLineRenderer(Border border)
        {
            this.border = border;
        }

        public void SavePNG([CallerMemberName]string fileName = null)
        {
            var rect = VisualTreeHelper.GetDescendantBounds(border);

            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen())
            {
                ctx.DrawRectangle(new VisualBrush(border), null, rect);
            }

            // Make a bitmap and draw on it.
            var width = (int)(rect.Width * scale);
            var height = (int)(rect.Height * scale);
            var rtb = new RenderTargetBitmap(width, height, 96 * scale, 96 * scale, PixelFormats.Pbgra32);
            rtb.Render(dv);

            // Make a PNG encoder.
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            // Save the file
            using (var fileStream = File.Create($"{fileName}.png"))
            {
                encoder.Save(fileStream);
            }
        }
    }
}
