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
        public string SourceImageStoredFileName { get; set; }
        [Parameter]
        public string WatermarkImageStoredFileName { get; set; }
        [Parameter]
        public int WatermarkStyle { get; set; }

        private WatermarkedImageDto WatermarkedImageDto { get; set; }
        private async Task RequestWatermarkedImage()
        {
            if (SourceImageStoredFileName == null || WatermarkImageStoredFileName == null)
                return;
            WatermarkedImageDto = await ImageService.RequestImage(new RequestImageDto
            {
                SourceImageStoredFileName = this.SourceImageStoredFileName,
                WatermarkImageStoredFileName = this.WatermarkImageStoredFileName,
                WatermarkStyle = WatermarkStyle
            });
        }
    }
}
