
using Microsoft.EntityFrameworkCore;

namespace WatermarkApi.DbContext;
public class DataDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
           
        }

        
    }

