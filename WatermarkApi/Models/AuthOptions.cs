using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WatermarkApi.Models;

public class AuthOptions
{

    public AuthOptions(string key, int lifetime)
    {
        Key = key;
        Lifetime = lifetime;
    }

    public AuthOptions()
    {

    }
    public string? Key { get; set; } = "";

    public int Lifetime { get; set; }

    public SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
}