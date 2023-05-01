using Kirel.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace WatermarkApi.Models;

public class User : IdentityUser<int>, ICreatedAtTrackedEntity, IKeyEntity<int>
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public DateTime Created { get; set; }
}