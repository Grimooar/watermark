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
        [Parameter]
        public int WatermarkStyle { get; set; }

        private WatermarkedImageDto WatermarkedImageDto { get; set; }
        private async Task RequestWatermarkedImage()
        {
            if ((WatermarkImage == null || WatermarkImage.ErrorCode != 0) ||
                (SourceImage == null || SourceImage.ErrorCode != 0))
                return;
            WatermarkedImageDto = await ImageService.RequestImage(new RequestImageDto
            {
                SourceImageStoredFileName = SourceImage.StoredFileName,
                WatermarkImageStoredFileName = WatermarkImage.StoredFileName,
                WatermarkStyle = WatermarkStyle
            });
        }
    }
}
