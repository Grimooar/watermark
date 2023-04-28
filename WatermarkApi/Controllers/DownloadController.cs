using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Watermark.Models.Dtos;
using WatermarkApi.Service;

namespace WatermarkApi.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DownloadController : ControllerBase
{
    private readonly IImageService _imageService;

    public DownloadController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("{imageId}/{watermarkId}")]
    public async Task<ActionResult<ResultImageDto>> Download(int imageId, int watermarkId)
    {
        var result = await _imageService.ApplyWatermarkAsync(imageId, watermarkId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(new ResultImageDto { ResultImageBaseString = Convert.ToBase64String(result) });
    }
}