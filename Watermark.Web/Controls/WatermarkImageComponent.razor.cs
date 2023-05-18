using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Watermark.Models.Dtos;
using Watermark.Web.Services.Contracts;

namespace Watermark.Web.Controls
{
    public partial class WatermarkImageComponent
    {
        [Parameter]
        public EventCallback<string> OnImageUploadedSuccess { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IImageService ImageService { get; set; }
        public UploadImagesDto WatermarkImage { get; set; }

        public string? ErrorMessage;
        public InputFile? inputWatermarkImageFile;
        public ElementReference previewWatermarkImageElem;

        private string? imageClass;

        private async Task UploadWatermarkImage(InputFileChangeEventArgs e)
        {
            ErrorMessage = null;
            if (WatermarkImage != null && WatermarkImage.ErrorMessage == null)
                await DeleteImage(WatermarkImage.StoredFileName);

            await JS.InvokeVoidAsync("previewImage", inputWatermarkImageFile!.Element, previewWatermarkImageElem);
            imageClass = "img-thumbnail";

            WatermarkImage = await UploadImage(e.File);
            if (ErrorMessage == null)
            {
                await OnImageUploadedSuccess.InvokeAsync(WatermarkImage.StoredFileName);
            }
        }
        private async Task<UploadImagesDto> UploadImage(IBrowserFile file)
        {
            try
            {
                var image = await ImageService.UploadImages(file);
                if (image.ErrorMessage != null)
                    ErrorMessage = image.ErrorMessage;
                return image;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await OnImageUploadedSuccess.InvokeAsync(null);
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
                await OnImageUploadedSuccess.InvokeAsync(null);
            }
        }
    }
}
