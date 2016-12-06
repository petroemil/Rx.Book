using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace TimelineGenerator
{
    public class TimeLineRenderer
    {
        private readonly Border border;
        private readonly double scale = 5;

        public TimeLineRenderer(Border border)
        {
            this.border = border;
        }

        public async Task SaveBMP([CallerMemberName]string fileName = null)
        {
            // Render the image
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(this.border, (int)(this.border.Width * this.scale), (int)(this.border.Height * this.scale));
            var pixels = await bitmap.GetPixelsAsync();

            // Set file location
            var rxFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("Rx", CreationCollisionOption.OpenIfExists);
            var file = await rxFolder.CreateFileAsync($"{fileName}.png");
            
            // Write the rendered image to the file
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96, 96, pixels.ToArray());
                await encoder.FlushAsync();
            }
        }
    }
}
