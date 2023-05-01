using Watermark.Models.Dtos;

namespace Watermark.Web.Services.Contracts
{
    public interface IAuthenticationService
    {
        Task<RegistrationResponseDto> RegisterUser(UserCreateDto userCreateDto);
        Task<AuthResponseDto> Login(LoginDto loginDto);
        Task Logout();
    }
}
