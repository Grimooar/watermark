using Watermark.Web.Services.Contracts;
using System.Net.Http.Json;
using Watermark.Models.Dtos;
using Microsoft.AspNetCore.Components.Forms;

namespace Watermark.Web.Services
{
    public class ImageService : IImageService
    {
        private readonly HttpClient httpClient;

        public ImageService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task DeleteImages(string storedFileName)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"api/Image/{storedFileName}");
                if (response.IsSuccessStatusCode)
                    return;
                return;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> RequestImage(string sourceImageStoredFileName, string watermarkImageStoredFileName)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Image/{sourceImageStoredFileName}/{watermarkImageStoredFileName}");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return (default);
                    }
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<UploadImagesDto> UploadImages(IBrowserFile browserFile)
        {
            try
            {
                var fileContent = new StreamContent(browserFile.OpenReadStream(maxAllowedSize: 512000 * 2));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(browserFile.ContentType);
                var content = new MultipartFormDataContent();
                content.Add(content: fileContent, name: "file", fileName: browserFile.Name);

                var response = await httpClient.PostAsync("api/Image", content);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(UploadImagesDto);
                    }
                    return await response.Content.ReadFromJsonAsync<UploadImagesDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode} - Message - {message}");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
