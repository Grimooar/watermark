using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Watermark.Models.Dtos;
using WatermarkApi.Models;

namespace WatermarkApi.Service
{
    public class AuthService
    {
        private readonly AuthOptions authOptions;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;

        public AuthService(IOptions<AuthOptions> authOptions, UserManager<User> userManager, IConfiguration configuration)
        {
            this.authOptions = authOptions.Value;
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task<string?> Login(LoginDto loginDto)
        {

            var user = await userManager.FindByNameAsync(loginDto.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
                return null;

            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return token;


        }

        private SigningCredentials GetSigningCredentials()
        {
            var authOptions = configuration.GetSection("AuthOptions").Get<AuthOptions>();
            var key = Encoding.UTF8.GetBytes(authOptions.Key);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private List<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(authOptions.Lifetime)),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        public async Task<IdentityResult> Register(UserCreateDto userCreate)
        {
            var user = new User
            {
                UserName = userCreate.Email,
                Name = userCreate.Name,
                LastName = userCreate.LastName,
                Created = DateTime.UtcNow,
                Email = userCreate.Email,
                PhoneNumber = userCreate.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, userCreate.Password);

            return result;
        }

    }
}

 