using Microsoft.AspNetCore.Components;
using Watermark.Models.Dtos;
using Watermark.Web.Services.Contracts;

namespace Watermark.Web.Controls
{
    public partial class ResultImageComponent
    {
        [Inject]
        public IImageService ImageService { get; set; }
        [Parameter]
        public UploadImagesDto SourceImage { get; set; }
        [Parameter]
        public UploadImagesDto WatermarkImage { get; set; }

        private string ResultImageBase64String { get; set; }
        private async Task RequestWatermarkedImage()
        {
            if ((WatermarkImage == null || WatermarkImage.ErrorCode != 0) ||
                (SourceImage == null || SourceImage.ErrorCode != 0))
                return;
            ResultImageBase64String = await ImageService.RequestImage(SourceImage.StoredFileName, WatermarkImage.StoredFileName);
        }
    }
}
