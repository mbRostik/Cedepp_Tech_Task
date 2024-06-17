using Cedepp.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Infrastructure.Data
{
    public class CedeppDbContext : DbContext
    {
        public CedeppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<UnRegisteredUser> UnRegisteredUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CedeppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
