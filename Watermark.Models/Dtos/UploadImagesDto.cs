using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watermark.Models.Dtos
{
    public class UploadImagesDto
    {
        public string SourceImageBaseString { get; set; }
        public string WatermarkImageBaseString { get; set; }
    }
}
