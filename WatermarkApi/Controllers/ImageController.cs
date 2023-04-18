// Controllers/ImageController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Watermark.Models.Dtos;
using WatermarkApi.Service;

namespace WatermarkApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<ActionResult<RequestImageDto>> UploadImages([FromBody] UploadImagesDto uploadImagesDto)
        {
            if ((uploadImagesDto.WatermarkImageBaseString == null || uploadImagesDto.WatermarkImageBaseString.Length == 0)
                || (uploadImagesDto.SourceImageBaseString == null || uploadImagesDto.SourceImageBaseString.Length == 0))
            {
                return BadRequest("File not provided or empty.");
            }

            byte[] sourceImageBytes = Convert.FromBase64String(uploadImagesDto.SourceImageBaseString);
            byte[] watermarkImageBytes = Convert.FromBase64String(uploadImagesDto.WatermarkImageBaseString);

            int sourceImageId = await _imageService.SaveImageAsync(sourceImageBytes);

            int watermarkImageId = await _imageService.SaveWatermarkAsync(watermarkImageBytes);

            return Ok(new RequestImageDto { SourceImageId = sourceImageId, WatermarkImageId = watermarkImageId });
        }
    }
}