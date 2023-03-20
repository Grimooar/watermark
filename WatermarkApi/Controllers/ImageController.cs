// Controllers/ImageController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using WatermarkApi.Service;

namespace WatermarkApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File not provided or empty.");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            int imageId = await _imageService.SaveImageAsync(memoryStream.ToArray());
            return Ok(new { ImageId = imageId });
        }
    }
}