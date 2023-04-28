// Controllers/ImageController.cs
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Watermark.Models.Dtos;
using WatermarkApi.Service;

namespace WatermarkApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<ImageController> logger;
        private readonly IImageService imageService;

        public ImageController(IWebHostEnvironment env, ILogger<ImageController> logger, IImageService imageService)
        {
            this.env = env;
            this.logger = logger;
            this.imageService = imageService;
        }
        
        [HttpPost]
        public async Task<ActionResult<UploadImagesDto>> UploadImages([FromForm] IFormFile file)
        {
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            var maxFileSize = 512000 * 2;

            var uploadResult = new UploadImagesDto();
            
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

            if (file.Length == 0)
            {
                logger.LogInformation($"{trustedFileNameForDisplay} length is 0 (Err: 1)");
                uploadResult.ErrorCode = 1;
            }
            else if (file.Length > maxFileSize)
            {
                logger.LogInformation($"{trustedFileNameForDisplay} of {file.Length} bytes is larger than the limit of {maxFileSize} bytes (Err: 2)");
                uploadResult.ErrorCode = 2;
            }
            else
            {
                await imageService.SaveImageToStorage(trustedFileNameForDisplay, uploadResult, file);
            }
            return new CreatedResult(resourcePath, uploadResult);
        }
        [HttpGet("{sourceImageStoredFileName}/{watermarkImageStoredFileName}")]
        public async Task<ActionResult> Download(string sourceImageStoredFileName, string watermarkImageStoredFileName)
        {
            var result = await imageService.ApplyWatermarkAsync(sourceImageStoredFileName, watermarkImageStoredFileName);
            if (result == null)
            {
                return NotFound();
            }
            var resultBase64String = Convert.ToBase64String(result);
            return Ok(resultBase64String);
        }
        [HttpDelete("{trustedFileName}")]
        public async Task<ActionResult> DeleteImage(string trustedFileNameForFileStorage)
        {
            try
            {
                var image = await imageService.DeleteImage(trustedFileNameForFileStorage);
                if (image == null)
                {
                    return NotFound();
                }

                var path = Path.Combine(env.ContentRootPath, env.EnvironmentName, "unsafe_uploads", trustedFileNameForFileStorage);
                FileInfo file = new FileInfo(path);
                file.Delete();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }
    }
}