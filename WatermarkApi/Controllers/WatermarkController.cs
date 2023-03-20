using Microsoft.AspNetCore.Mvc;
using WatermarkApi.Models;
using WatermarkApi.Service;

namespace WatermarkApi.Controllers;
[ApiController]
[Route("[controller]")]
public class WatermarkController : Controller
{
    private readonly IImageService _imageService;
    
    
    public WatermarkController(IImageService imageService)
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

        int imageId = await _imageService.SaveWatermarkAsync(memoryStream.ToArray());
        return Ok(new { ImageId = imageId });
    }
}