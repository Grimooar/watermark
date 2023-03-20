using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats.Jpeg;
using WatermarkApi.DbContext;
using WatermarkApi.Models;

namespace WatermarkApi.Service
{
    public class ImageService : IImageService
    {
        private readonly DataDbContext _context;

        public ImageService(DataDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveImageAsync(byte[] imageBytes)
        {
            var image = new SourceImage { Data = imageBytes };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return image.Id;
        }

      

        public async Task<int> SaveWatermarkAsync(byte[] watermarkBytes)
        {
            var watermark = new WatermarkImage { Data = watermarkBytes };
            _context.Watermarks.Add(watermark);
            await _context.SaveChangesAsync();

            return watermark.Id;
        }

        public async Task<byte[]> ApplyWatermarkAsync(int imageId, int watermarkId)
        {
            var image = await _context.Images.FindAsync(imageId);
            var watermark = await _context.Watermarks.FindAsync(watermarkId);

            if (image == null || watermark == null)
            {
                return null;
            }

            using var resultStream = new MemoryStream();
    
            // Load image and watermark into Bitmap objects
            using var imageBitmap = new Bitmap(new MemoryStream(image.Data));
            using var watermarkBitmap = new Bitmap(new MemoryStream(watermark.Data));
    
            // Create a new Bitmap with the same dimensions as the original image
            using var resultBitmap = new Bitmap(imageBitmap.Width, imageBitmap.Height);

            // Create a new Graphics object from the new Bitmap
            using var graphics = Graphics.FromImage(resultBitmap);

            // Draw the original image onto the new Bitmap
            graphics.DrawImage(imageBitmap, 0, 0);

            // Determine the position of the watermark in the bottom-right corner of the image
            var watermarkX = resultBitmap.Width - watermarkBitmap.Width;
            var watermarkY = resultBitmap.Height - watermarkBitmap.Height;

            // Draw the watermark onto the new Bitmap
            graphics.DrawImage(watermarkBitmap, watermarkX, watermarkY, watermarkBitmap.Width, watermarkBitmap.Height);

            // Save the new Bitmap to the stream as a JPEG
            resultBitmap.Save(resultStream, ImageFormat.Jpeg);

            return resultStream.ToArray();
        }

       

    }
}