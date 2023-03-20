using System.Drawing;

namespace WatermarkApi.Models
{
    public class SourceImage
    {
        public int Id { get; set; }
        public Bitmap Image { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
