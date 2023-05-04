using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watermark.Bot.Models;

namespace Watermark.Bot.Data
{
    public class UserDataDbContext : DbContext
    {
        public UserDataDbContext(DbContextOptions<UserDataDbContext> options) : base(options)
        {
            
        }

        public DbSet<UserData> UserDatas { get; set; }
    }
}
