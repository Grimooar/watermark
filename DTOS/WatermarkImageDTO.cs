using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOS
{
    internal class WatermarkImageDTO
    {
        public int Id { get; set; }
        public Bitmap Image {  get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
