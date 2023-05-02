using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Watermark.Models.Dtos;
using Watermark.Web.Services.Contracts;
using Microsoft.JSInterop;

namespace Watermark.Web.Pages
{
    public partial class WatermarkComponent : ComponentBase
    {
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IImageService ImageService { get; set; }

        private int WatermarkStyle { get; set; } = 1;
        private UploadImagesDto SourceImage { get; set; }
        private UploadImagesDto WatermarkImage { get; set; }
        private string ErrorMessage { get; set; }

        private InputFile? inputSourceImageFile;
        private InputFile? inputWatermarkImageFile;
        private ElementReference previewSourceImageElem;
        private ElementReference previewWatermarkImageElem;

        private async Task UploadSourceImage(InputFileChangeEventArgs e)
        {
            if (SourceImage != null && SourceImage.ErrorCode == 0)
                await DeleteImage(SourceImage.StoredFileName);

            await JS.InvokeVoidAsync("previewImage", inputSourceImageFile!.Element, previewSourceImageElem);
            SourceImage = await UploadImage(e.File);

        }
        private async Task UploadWatermarkImage(InputFileChangeEventArgs e)
        {
            if (WatermarkImage != null && WatermarkImage.ErrorCode == 0)
                await DeleteImage(WatermarkImage.StoredFileName);

            await JS.InvokeVoidAsync("previewImage", inputWatermarkImageFile!.Element, previewWatermarkImageElem);
            WatermarkImage = await UploadImage(e.File);
        }
        private async Task<UploadImagesDto> UploadImage(IBrowserFile file)
        {
            try
            {
                var image = await ImageService.UploadImages(file);
                if (image.ErrorCode != 0)
                    await HandleImageUploadError(image.ErrorCode);
                return image;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return default(UploadImagesDto);
        }
        private async Task DeleteImage(string storedFileName)
        {
            try
            {
                await ImageService.DeleteImages(storedFileName);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        private async Task HandleImageUploadError(int errorCode)
        {
            switch (errorCode)
            {
                case 1: 
                    ErrorMessage = "Error 1: No Image was uploaded";
                    break;
                case 2:
                    ErrorMessage = "Error 2: File is too big";
                    break;
                case 3:
                    ErrorMessage = "Error 3: Internal server error";
                    break;
                case 4: 
                    ErrorMessage = "Error 4: Uploaded file is unsupported type";
                    break;
            }
        }
    }
}
