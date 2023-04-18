using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOS.Dtos
{
    public class UploadImagesDto
    {
        public FileStream SourceImage { get; set; }
        public FileStream WatermarkImage { get; set; }
    }
}
