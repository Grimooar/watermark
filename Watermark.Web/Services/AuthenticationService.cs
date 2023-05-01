using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Watermark.Models.Dtos;
using Watermark.Web.AuthProviders;
using Watermark.Web.Services.Contracts;

namespace Watermark.Web.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient httpClient;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ILocalStorageService localStorageService;
        private readonly JsonSerializerOptions options;
        public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorageService)
        {
            this.httpClient = httpClient;
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorageService = localStorageService;
            options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var content = JsonSerializer.Serialize(loginDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var authResult = await httpClient.PostAsync("api/Auth/login", bodyContent);
            var authContent = await authResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponseDto>(authContent, options);

            if (!authResult.IsSuccessStatusCode)
                return result;

            await localStorageService.SetItemAsync("authToken", result.Token);
            ((CustomAuthStateProvider)authenticationStateProvider).NotifyUserAuthentication(loginDto.Email);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);

            return new AuthResponseDto { IsAuthSuccessful = true };
        }

        public async Task Logout()
        {
            await localStorageService.RemoveItemAsync("authToken");
            ((CustomAuthStateProvider)authenticationStateProvider).NotifyUserLogout();
            httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<RegistrationResponseDto> RegisterUser(UserCreateDto userCreateDto)
        {
            var content = JsonSerializer.Serialize(userCreateDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var registrationResult = await httpClient.PostAsync("api/Auth/register", bodyContent);
            var registrationContent = await registrationResult.Content.ReadAsStringAsync();

            if (!registrationResult.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<RegistrationResponseDto>(registrationContent, options);
                return result;
            }

            return new RegistrationResponseDto { IsSuccessfulRegistraion = true };
        }
    }
}
