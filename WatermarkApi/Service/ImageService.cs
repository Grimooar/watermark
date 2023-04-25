using System.Drawing;
using System.Drawing.Imaging;
using WatermarkApi.DbContext;
using WatermarkApi.Models;

namespace WatermarkApi.Service
{
    public class ImageService : IImageService
    {
        private readonly DataDbContext _context;
        private readonly IWebHostEnvironment env;

        public ImageService(DataDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        public async Task SaveImageToDb(string storedFileName)
        {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddSeconds(20); //AddHours(6)
            var image = new StoredImage { StoredName = storedFileName, ExpireDateTime = dateTime };
            var result = _context.StoredImages.Add(image);
            await _context.SaveChangesAsync();
            return;
        }
        public async Task<StoredImage> DeleteImage(string storedFileName)
        {
            var image = await _context.StoredImages.FindAsync(storedFileName);

            if (image != null)
            {
                _context.StoredImages.Remove(image);
                await _context.SaveChangesAsync();
            }

            return image;
        }
        public async Task DeleteExpiredImages()
        {
            var path = Path.Combine(env.ContentRootPath, env.EnvironmentName, "unsafe_uploads");
            var expiredImages = _context.StoredImages.Where(x => x.ExpireDateTime <= DateTime.Now);
            if (!expiredImages.Any())
                return;

            foreach (var expiredImage in expiredImages)
            {
                var fullPath = Path.Combine(path, expiredImage.StoredName);
                FileInfo file = new FileInfo(fullPath);
                file.Delete();
                _context.StoredImages.Remove(expiredImage);
            }
            await _context.SaveChangesAsync();
            
        }
/*        public async Task<int> SaveImageAsync(byte[] imageBytes)
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
        }*/

       /* public async Task<byte[]> ApplyWatermarkAsync(int imageId, int watermarkId)
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

            //Ќанесение вод€ного знака по всей картинке
            for (int watermarkY = 0; watermarkY <= resultBitmap.Height; watermarkY += watermarkBitmap.Height)
            {
                for (int watermarkX = 0; watermarkX <= resultBitmap.Width; watermarkX += watermarkBitmap.Width)
                {
                    graphics.DrawImage(watermarkBitmap, watermarkX, watermarkY, watermarkBitmap.Width, watermarkBitmap.Height);
                }
            }
            // Determine the position of the watermark in the bottom-right corner of the image
            *//*var watermarkX = resultBitmap.Width - watermarkBitmap.Width;
            var watermarkY = resultBitmap.Height - watermarkBitmap.Height;*//*

            // Draw the watermark onto the new Bitmap
            *//*graphics.DrawImage(watermarkBitmap, watermarkX, watermarkY, watermarkBitmap.Width, watermarkBitmap.Height);*//*

            // Save the new Bitmap to the stream as a JPEG
            resultBitmap.Save(resultStream, ImageFormat.Jpeg);

            return resultStream.ToArray();
        }*/

       

    }
}