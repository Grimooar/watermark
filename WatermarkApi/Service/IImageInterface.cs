using Watermark.Models.Dtos;
using WatermarkApi.Models;

namespace WatermarkApi.Service;

public interface IImageService
{
    Task SaveImageToStorage(string trustedFileNameForDisplay, UploadImagesDto uploadResult, IFormFile file);
    Task SaveImageToDb(string storedFileName);
    Task<StoredImage> DeleteImage(string storedFileName);
    Task DeleteExpiredImages();
    /*Task<int> SaveImageAsync(byte[] imageBytes);
    Task<int> SaveWatermarkAsync(byte[] watermarkBytes);*/
    Task<byte[]> ApplyWatermarkAsync(RequestImageDto requestImageDto);
}