using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Watermark.Models.Dtos;
using WatermarkApi.Models;
using WatermarkApi.Service;

namespace WatermarkApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService authService;
        private readonly UserManager<User> userManager;

        public AuthController(AuthService authService, UserManager<User> userManager)
        {
            this.authService = authService;
            this.userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto userCreate)
        {
            if (userCreate == null || !ModelState.IsValid)
                return BadRequest();

            var result = await authService.Register(userCreate);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }

            return StatusCode(201);
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await authService.Login(loginDto);

            if (token == null)
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });

            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }
    }
}