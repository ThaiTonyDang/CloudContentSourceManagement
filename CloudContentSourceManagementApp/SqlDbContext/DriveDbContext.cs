using CloudContentSourceManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudContentSourceManagementApp.SqlDbContext
{
    class DriveDbContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<GoogleAppProfile> GoogleAppProfiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!Directory.Exists(Program.AppFolder))
                Directory.CreateDirectory(Program.AppFolder);

            optionsBuilder.UseSqlite($"Data Source={Program.DbUser}");
            optionsBuilder.UseSqlite($"Data Source={Program.DbAppProfile}");
        }

    }
}
