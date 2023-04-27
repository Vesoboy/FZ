using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using FZ.WriteLogs;
using System;

namespace FZ.DB
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }
        public DataContext(DbContextOptions options) : base(options)
        {
            if (!(this.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                Database.EnsureCreated();
            }
        }

        public virtual DbSet<DataBaseSite> Site { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var rootGuid = Guid.NewGuid();
            modelBuilder.Entity<DataBaseSite>(b =>
            {
                b.HasData(new DataBaseSite
                {
                    Id = rootGuid,
                    url = "root",
                    RetryCount = 0,
                    message = "root",
                });
            });

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SiteBD_Log;Username=developer_user;Password=developer_user");
        }
    }
}
