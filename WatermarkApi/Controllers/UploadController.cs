using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WatermarkApi.Controllers
{
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<JsonResult> Upload()
        {
            string __filepath = Path.Combine(_environment.WebRootPath, "uploads");
            int __maxSize = 2 * 1024 * 1024; // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result
            {
                Files = new List<string>()
            };

            IFormFileCollection files = Request.Form.Files;

            if (files.Count > 0)
            {
                foreach (var file in files)
                {
                    // Выполнить проверки на допустимый размер файла и формат
                    if (file.Length > __maxSize)
                    {
                        result.Error = "Размер файла не должен превышать 2 Мб";
                        break;
                    }
                    else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                    {
                        result.Error = "Недопустимый формат файла";
                        break;
                    }

                    // Сохранить файл и вернуть URL
                    if (!Directory.Exists(__filepath))
                        Directory.CreateDirectory(__filepath);

                    Guid guid = Guid.NewGuid();
                    string fileName = $"{guid}.{file.FileName}";
                    string filePath = Path.Combine(__filepath, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream);

                    result.Files.Add($"/uploads/{fileName}");
                }
            }

            return Json(result);
        }
    }

    public class Result
    {
        public string Error { get; set; }
        public List<string> Files { get; set; }
    }
}