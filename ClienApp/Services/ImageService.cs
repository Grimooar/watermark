﻿using BlazorApp1.Services.Contracts;
using DTOS.Dtos;
using System.Net.Http.Json;

namespace BlazorApp1.Services
{
    public class ImageService : IImageService
    {
        private readonly HttpClient httpClient;

        public ImageService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<FileStream> DowmloadImages(int sourceImageId, int watermarkImageId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Download/{sourceImageId}/{watermarkImageId}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    return await response.Content.ReadFromJsonAsync<FileStream>();
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

        public async Task<ResultImageDto> UploadImages(UploadImagesDto uploadImagesDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync<UploadImagesDto>("api/Image", uploadImagesDto);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(ResultImageDto);
                    }
                    return await response.Content.ReadFromJsonAsync<ResultImageDto>();
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