using Microsoft.AspNetCore.Components;
using Watermark.Models.Dtos;
using Watermark.Web.Services.Contracts;

namespace Watermark.Web.Pages
{
    public partial class LoginComponent
    {
        private LoginDto loginDto = new LoginDto();

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private bool ShowAuthError { get; set; }
        private string Error { get; set; }

        public async Task ExecuteLogin()
        {
            ShowAuthError = false;
            
            var result = await AuthenticationService.Login(loginDto);
            if (!result.IsAuthSuccessful)
            {
                Error = result.ErrorMessage;
                ShowAuthError = true;
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }
        }
    }
}
