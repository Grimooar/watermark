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
         
            return resultStream.ToArray();
        }
       

    }
}