using Kirel.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace WatermarkApi.Models;

public class User : IdentityUser<int>, ICreatedAtTrackedEntity, IKeyEntity<int>
{
    public int Id { get; set; }
    public override string? UserName { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public new string? Email { get; set; }
    public DateTime Created { get; set; }
    public string? Password { get; set; }
}