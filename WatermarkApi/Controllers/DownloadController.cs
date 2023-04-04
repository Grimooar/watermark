using Microsoft.AspNetCore.Mvc;
using WatermarkApi.Service;

namespace WatermarkApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DownloadController : ControllerBase
{
    private readonly IImageService _imageService;

    public DownloadController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("{imageId}/{watermarkId}")]
    public async Task<IActionResult> Download(int imageId, int watermarkId)
    {
        var result = await _imageService.ApplyWatermarkAsync(imageId, watermarkId);
        if (result == null)
        {
            return NotFound();
        }
        return File(result, "image/jpeg");
    }
}