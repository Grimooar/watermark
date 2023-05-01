using Microsoft.AspNetCore.Components;
using Watermark.Web.Services.Contracts;

namespace Watermark.Web.Pages
{
    public partial class LogoutComponent
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AuthenticationService.Logout();
            NavigationManager.NavigateTo("/");
        }
    }
}
