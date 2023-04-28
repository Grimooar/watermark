using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watermark.Models.Dtos
{
    public class RequestImageDto
    {
        public string SourceImageStoredFileName { get; set; }
        public string WatermarkImageStoredFileName { get; set; }
    }
}
