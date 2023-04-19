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

        public ImageController(IWebHostEnvironment env, ILogger<ImageController> logger)
        {
            this.env = env;
            this.logger = logger;
        }
        private async Task SaveImageToStorage(string trustedFileNameForDisplay, UploadImagesDto uploadResult, IFormFile file)
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
            }
            catch (IOException ex)
            {
                logger.LogError($"{trustedFileNameForDisplay} error in upload (Err: 3): {ex.Message}");
                uploadResult.ErrorCode = 3;
            }
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
                await SaveImageToStorage(trustedFileNameForDisplay, uploadResult, file);
            }
            return new CreatedResult(resourcePath, uploadResult);
        }
        [HttpDelete("{trustedFileName}")]
        public async Task<ActionResult> DeleteImage(string trustedFileName)
        {
            var path = Path.Combine(env.ContentRootPath, env.EnvironmentName, "unsafe_uploads", trustedFileName);
            FileInfo file = new FileInfo(path);
            file.Delete();

            return Ok();
        }
    }
}