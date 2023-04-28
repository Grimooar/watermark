using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WatermarkApi.Models;

public class AuthOptions
{

    public AuthOptions(string issuer, string audience, string key, int lifetime)
    {
        Issuer = issuer;
        Audience = audience;
        Key = key;
        Lifetime = lifetime;
    }

    public AuthOptions()
    {
    }

    public string? Issuer { get; set; } = "";

    public string? Audience { get; set; } = "";

    public string? Key { get; set; } = "";

    public int Lifetime { get; set; }

    public SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
}