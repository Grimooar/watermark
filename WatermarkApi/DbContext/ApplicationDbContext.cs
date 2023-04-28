using Microsoft.EntityFrameworkCore;
using WatermarkApi.Models;

namespace WatermarkApi.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
        
    }
    public DbSet<User> User { get; set; }
    
}