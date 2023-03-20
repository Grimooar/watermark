using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace WatermarkApi.GenericRepository;





    public static class TextWaterMark
    {
        /// <summary>
        /// Метод, подготавливающий поверхность для рисования GDI+
        /// </summary>
        /// <returns>Возвращает объект Graphics</returns>
        private static Graphics GdiBase(
            HttpPostedFileBase file, 
            ref Bitmap bitmap,
            out int imageWidth, out int imageHeight)
        {
            // Получить изображение, его ширину и высоту, преобразовать в объект Bitmap
            Image image = Image.FromStream(file.InputStream, true, true);
            imageWidth = image.Width;
            imageHeight = image.Height;

            bitmap = new Bitmap(imageWidth, imageHeight,
                PixelFormat.Format24bppRgb);

            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            // Базовый класс GDI+, создающий слой для рисования
            Graphics graphics = Graphics.FromImage(bitmap);

            // Рисуем картинку
            graphics.DrawImage(
                image,
                new Rectangle(0, 0, imageWidth, imageHeight),
                0,
                0,
                imageWidth,
                imageHeight,
                GraphicsUnit.Pixel);

            return graphics;
        }

        // …
    }
