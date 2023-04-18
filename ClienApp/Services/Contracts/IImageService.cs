using DTOS.Dtos;
namespace BlazorApp1.Services.Contracts
{
    public interface IImageService
    {
        Task<ResultImageDto> UploadImages(UploadImagesDto uploadImagesDto);
        Task<FileStream> DowmloadImages(int sourceImageId, int watermarkImageId);

    }
}
