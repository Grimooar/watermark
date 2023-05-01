using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watermark.Models.Dtos
{
    public class RequestImageDto
    {
        [Required]
        public string SourceImageStoredFileName { get; set; }
        [Required]
        public string WatermarkImageStoredFileName { get; set; }
    }
}
