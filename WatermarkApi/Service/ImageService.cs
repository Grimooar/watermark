using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Watermark.Models.Dtos;
using WatermarkApi.DbContext;
using WatermarkApi.Models;

namespace WatermarkApi.Service
{
    public class ImageService : IImageService
    {
        private readonly DataDbContext _context;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<ImageService> logger;

        public ImageService(DataDbContext context, IWebHostEnvironment env, ILogger<ImageService> logger)
        {
            _context = context;
            this.env = env;
            this.logger = logger;
        }
        public async Task SaveImageToStorage(string trustedFileNameForDisplay, UploadImagesDto uploadResult, IFormFile file)
        {
            try
            {
                string trustedFileNameForFileStorage;
                trustedFileNameForFileStorage = Path.GetRandomFileName();
                var path = Path.Combine(env.ContentRootPath, env.EnvironmentName, "unsafe_uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, trustedFileNameForFileStorage);
                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                logger.LogInformation($"{trustedFileNameForDisplay} saved at {path}");
                uploadResult.Uploaded = true;
                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                await SaveImageToDb(trustedFileNameForFileStorage);
            }
            catch (IOException ex)
            {
                logger.LogError($"{trustedFileNameForDisplay} error in upload (Err: 3): {ex.Message}");
                uploadResult.ErrorMessage = "Внутрішня помилка серверу";
            }
        }
        public async Task SaveImageToDb(string storedFileName)
        {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddHours(6); //AddHours(6)
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
        
        public async Task<byte[]> ApplyWatermarkAsync(RequestImageDto requestImageDto)
        {
            var path = Path.Combine(env.ContentRootPath, env.EnvironmentName, "unsafe_uploads");
            FileInfo sourceImageFileInfo = new FileInfo(Path.Combine(path, requestImageDto.SourceImageStoredFileName));
            FileInfo watermarkImageFileInfo = new FileInfo(Path.Combine(path, requestImageDto.WatermarkImageStoredFileName));
            if (!sourceImageFileInfo.Exists || !watermarkImageFileInfo.Exists)
            {
                return null;
            }
            byte[] sourceImage = new byte[sourceImageFileInfo.Length];
            byte[] watermarkImage = new byte[watermarkImageFileInfo.Length];

            using (FileStream fs = sourceImageFileInfo.OpenRead())
            {
                fs.Read(sourceImage, 0, sourceImage.Length);
            }
            using (FileStream fs = watermarkImageFileInfo.OpenRead())
            {
                fs.Read(watermarkImage, 0, watermarkImage.Length);
            }

            using var resultStream = new MemoryStream();

            // Load image and watermark into Bitmap objects
            using var sourceImageBitmap = new Bitmap(new MemoryStream(sourceImage));
            using var watermarkBitmap = new Bitmap(new MemoryStream(watermarkImage));

            // Create a new Bitmap with the same dimensions as the original image
            using var resultBitmap = new Bitmap(sourceImageBitmap.Width, sourceImageBitmap.Height);

            // Create a new Graphics object from the new Bitmap
            using var graphics = Graphics.FromImage(resultBitmap);

            // Draw the original image onto the new Bitmap
            graphics.DrawImage(sourceImageBitmap, 0, 0, sourceImageBitmap.Width, sourceImageBitmap.Height);

            //Resize watermark to 1/10 of source image
            int watermarkNewWidth = sourceImageBitmap.Width / 3;
            int watermarkNewHeight = (int)((float)watermarkBitmap.Height * ((float)watermarkNewWidth / (float) watermarkBitmap.Width));
            var resizedWatermarkBitmap = ResizeImage(watermarkBitmap, watermarkNewWidth, watermarkNewHeight);
            
            //Apply watermark to souce image in selected location
            switch (requestImageDto.WatermarkStyle)
            {
                case 1:
                    await ApplyWatermarkAllOverImage(resultBitmap, resizedWatermarkBitmap, graphics);
                    break;
                case var n when n >= 2 && n <= 3:
                    await ApplyWatermarkInCorner(resultBitmap, resizedWatermarkBitmap, graphics, n);
                    break;
            }

            // Save the new Bitmap to the stream as a JPEG
            resultBitmap.Save(resultStream, ImageFormat.Png);

            return resultStream.ToArray();
        }

        private async Task ApplyWatermarkAllOverImage(Bitmap resultBitmap, Bitmap watermarkBitmap, Graphics graphics)
        {
            for (int watermarkY = 0; watermarkY <= resultBitmap.Height; watermarkY += watermarkBitmap.Height)
            {
                for (int watermarkX = 0; watermarkX <= resultBitmap.Width; watermarkX += watermarkBitmap.Width)
                {
                    graphics.DrawImage(watermarkBitmap, watermarkX, watermarkY, watermarkBitmap.Width, watermarkBitmap.Height);
                }
            }
        }
        private async Task ApplyWatermarkInCorner(Bitmap resultBitmap, Bitmap watermarkBitmap, Graphics graphics, int watermarkPos)
        {
            int watermarkX, watermarkY;
            switch (watermarkPos)
            {
                case 2: //Bottom right corner
                    watermarkX = resultBitmap.Width - watermarkBitmap.Width;
                    watermarkY = resultBitmap.Height - watermarkBitmap.Height;
                    break;
                case 3: //Upper left corner
                    watermarkX = 0;
                    watermarkY = 0;
                    break;
                default:
                    watermarkX = default(int);
                    watermarkY = default(int);
                    break;
            }

            graphics.DrawImage(watermarkBitmap, watermarkX, watermarkY, watermarkBitmap.Width, watermarkBitmap.Height);
        }

        private static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.Width, image.Height);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.PixelOffsetMode = PixelOffsetMode.Default;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }



    }
}