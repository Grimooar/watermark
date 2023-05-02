using Watermark.Web.Services.Contracts;
using System.Net.Http.Json;
using Watermark.Models.Dtos;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json;
using System.Text;

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

        public async Task<WatermarkedImageDto> RequestImage(RequestImageDto requestImageDto)
        {
            try
            {
                var content = JsonSerializer.Serialize(requestImageDto);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

                var requestResult = await httpClient.PostAsync("api/Image/requestImage", bodyContent);
                var requestContent = await requestResult.Content.ReadFromJsonAsync<WatermarkedImageDto>();

                if (requestResult.IsSuccessStatusCode)
                    return requestContent;
                else
                {
                    var message = requestContent;
                    throw new Exception($"Http staus code - {requestResult.StatusCode}, Message - {message}");
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
                var fileContent = new StreamContent(browserFile.OpenReadStream(maxAllowedSize: 1000000 * 5));
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
