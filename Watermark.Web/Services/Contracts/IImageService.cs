using Watermark.Models.Dtos;

namespace Watermark.Web.Services.Contracts
{
    public interface IImageService
    {
        Task<RequestImageDto> UploadImages(UploadImagesDto uploadImagesDto);
        Task<ResultImageDto> DowmloadImages(int sourceImageId, int watermarkImageId);

    }
}
