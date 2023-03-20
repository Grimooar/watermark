namespace WatermarkApi.Service;

public interface IImageService
{
    Task<int> SaveImageAsync(byte[] imageBytes);
    Task<int> SaveWatermarkAsync(byte[] watermarkBytes);
    Task<byte[]> ApplyWatermarkAsync(int imageId, int watermarkId);
}