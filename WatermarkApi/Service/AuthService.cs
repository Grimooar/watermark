using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Watermark.Models.Dtos;
using WatermarkApi.Models;
using WebApi.Service;

namespace WatermarkApi.Service;

 public class AuthService
    {
        private readonly AuthOptions _authOptions;
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public AuthService(IOptions<AuthOptions> authOptions, UserManager<User> userManager, UserService userService,IConfiguration configuration)
        {
            _authOptions = authOptions.Value;
            _userManager = userManager;
            _userService = userService;
            _configuration = configuration;
        }
        public async Task<string?> Login(LoginDto loginDto)
        {
        
            var user = await _userManager.FindByIdAsync(loginDto.Id);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return null;
            }

// Create a list of claims to include in the JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

// Generate a JWT using the provided authentication options
            var authOptions = _configuration.GetSection("AuthOptions").Get<AuthOptions>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(authOptions.Issuer, authOptions.Audience, claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(authOptions.Lifetime)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

           
        }
        // public async Task<string?> Login(LoginDto loginDto)
        // {
        //    //var user = await _userService.GetUserByEmailAsync(loginDto.Email);
        //      var user = await _userManager.FindByEmailAsync(loginDto.Email);
        //     if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        //         return null;
        //
        //     var claims = new List<Claim>
        //     {
        //         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //         new Claim(ClaimTypes.Email, user.Email),
        //         new Claim(ClaimTypes.Name, user.UserName)
        //     };
        //
        //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Key));
        //     var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //     var token = new JwtSecurityToken(_authOptions.Issuer, _authOptions.Audience, claims,
        //         expires: DateTime.Now.AddMinutes(Convert.ToDouble(_authOptions.Lifetime)), signingCredentials: credentials);
        //
        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }


        public async Task<IdentityResult> Register(UserDto userDto)
        {
            var user = new User
            {
                UserName = userDto.UserName,
                
                
                Name = userDto.Name,
                LastName = userDto.LastName,
                Created = DateTime.UtcNow,
                Email = userDto.Email
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            return result;
        }

    }