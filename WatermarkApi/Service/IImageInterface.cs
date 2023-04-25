using WatermarkApi.Models;

namespace WatermarkApi.Service;

public interface IImageService
{
    Task SaveImageToDb(string storedFileName);
    Task<StoredImage> DeleteImage(string storedFileName);
    Task DeleteExpiredImages();
    /*Task<int> SaveImageAsync(byte[] imageBytes);
    Task<int> SaveWatermarkAsync(byte[] watermarkBytes);*/
    Task<byte[]> ApplyWatermarkAsync(int imageId, int watermarkId);
}