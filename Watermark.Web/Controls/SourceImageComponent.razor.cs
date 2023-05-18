using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Watermark.Models.Dtos;
using Watermark.Web.Services.Contracts;

namespace Watermark.Web.Controls
{
    public partial class SourceImageComponent
    {
        [Parameter]
        public EventCallback<string> OnImageUploadedSuccess { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IImageService ImageService { get; set; }

        public UploadImagesDto SourceImage { get; set; }

        public InputFile? inputSourceImageFile;

        public ElementReference previewSourceImageElem;
        public string? ErrorMessage;

        private string? imageClass;
        private async Task UploadSourceImage(InputFileChangeEventArgs e)
        {
            ErrorMessage = null;
            if (SourceImage != null && SourceImage.ErrorMessage == null)
                await DeleteImage(SourceImage.StoredFileName);

            await JS.InvokeVoidAsync("previewImage", inputSourceImageFile!.Element, previewSourceImageElem);
            imageClass = "img-thumbnail";

            SourceImage = await UploadImage(e.File);
            if (ErrorMessage == null)
            {
                await OnImageUploadedSuccess.InvokeAsync(SourceImage.StoredFileName);
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
                await OnImageUploadedSuccess.InvokeAsync(null);
                ErrorMessage = ex.Message;
            }
        }
    }
}
