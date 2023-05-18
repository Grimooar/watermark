using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Watermark.Models.Dtos;
using Watermark.Web.Services.Contracts;
using Microsoft.JSInterop;
using Watermark.Web.Controls;

namespace Watermark.Web.Pages
{
    public partial class WatermarkComponent : ComponentBase
    {
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IImageService ImageService { get; set; }

        private string? sourceImageStoredFileName;
        private string? watermarkImageStoredFileName;
        private int watermarkStyle = 1;

        private void OnSourceImageFileNameChanged(string value)
        {
            sourceImageStoredFileName = value;
        }
        private void OnWatermarkImageFileNameChanged(string value)
        {
            watermarkImageStoredFileName = value;
        }
    }
}
