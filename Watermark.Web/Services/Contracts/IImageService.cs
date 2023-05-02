using Microsoft.AspNetCore.Components.Forms;
using Watermark.Models.Dtos;

namespace Watermark.Web.Services.Contracts
{
    public interface IImageService
    {
        Task<UploadImagesDto> UploadImages(IBrowserFile file);
        Task<WatermarkedImageDto> RequestImage(RequestImageDto requestImageDto);
        Task DeleteImages(string storedFileName);

    }
}
