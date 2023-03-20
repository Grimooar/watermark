using System.Drawing;
using System.Net.Mime;

namespace WatermarkApi.Examples;

public class Help
{
    static Image AddWaterMark(Image orig, Image waterMark, Point location) {
        Image result = new Bitmap(orig);
        using (Graphics g = Graphics.FromImage(result)) {
            g.DrawImage(waterMark, location);
        }
        return result;
    }
    
    /* // Получить изображение, его ширину и высоту, преобразовать в объект Bitmap
            Image image = Image.FromStream(file.InputStream, true, true);
            imageWidth = image.Width;
            imageHeight = image.Height;*/
}