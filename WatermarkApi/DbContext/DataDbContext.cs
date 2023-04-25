using Microsoft.EntityFrameworkCore;
using WatermarkApi.Models;

namespace WatermarkApi.DbContext;
public class DataDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
           
        }
        public DbSet<StoredImage> StoredImages { get; set; }
    }

