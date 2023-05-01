using Microsoft.AspNetCore.Components;
using Watermark.Models.Dtos;
using Watermark.Web.Services.Contracts;

namespace Watermark.Web.Pages
{
    public partial class RegistrationComponent
    {
        private UserCreateDto userCreateDto = new UserCreateDto();
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private bool ShowRegistrationErrors { get; set; }
        private IEnumerable<string> Errors { get; set; }

        private async Task Register()
        {
            ShowRegistrationErrors = false;
            
            var result = await AuthenticationService.RegisterUser(userCreateDto);
            if (!result.IsSuccessfulRegistraion)
            {
                Errors = result.Errors;
                ShowRegistrationErrors = true;
            }
            else NavigationManager.NavigateTo("/");
        }
    }
}
