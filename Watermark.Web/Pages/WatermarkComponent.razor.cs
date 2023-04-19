using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Watermark.Models.Dtos;
using Watermark.Web.Services.Contracts;

namespace Watermark.Web.Pages
{
    public partial class WatermarkComponent : ComponentBase
    {
        [Inject]
        public IImageService ImageService { get; set; }

        private UploadImagesDto SourceImage { get; set; }
        private UploadImagesDto WatermarkImage { get; set; }
        private string ErrorMessage { get; set; }

        private async Task UploadSourceImage(InputFileChangeEventArgs e)
        {
            if (SourceImage != null && SourceImage.ErrorCode == 0)
            {
                try
                {
                    await ImageService.DeleteImages(SourceImage.StoredFileName);
                }
                catch (Exception ex)
                {

                    ErrorMessage = ex.Message;
                }
            }
            try
            {
                SourceImage = await ImageService.UploadImages(e.File);
                if (SourceImage.ErrorCode != 0)
                {
                    ErrorMessage = SourceImage.ErrorCode.ToString();
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }


        }
        private async Task UploadWatermarkImage(InputFileChangeEventArgs e)
        {
            if (SourceImage != null && SourceImage.ErrorCode == 0)
            {
                try
                {
                    await ImageService.DeleteImages(SourceImage.StoredFileName);
                }
                catch (Exception ex)
                {

                    ErrorMessage = ex.Message;
                }
            }
            try
            {
                WatermarkImage = await ImageService.UploadImages(e.File);
                if (WatermarkImage.ErrorCode != 0)
                {
                    ErrorMessage = WatermarkImage.ErrorCode.ToString();
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }


        }
    }
}
