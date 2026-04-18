using GasApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GasApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<AlertAudit> AlertAudit { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Alert>()
                .Property(a => a.Status)
                .HasMaxLength(50);

            modelBuilder.Entity<Technician>()
                .Property(t => t.FullName)
                .HasMaxLength(100);

            modelBuilder.Entity<AlertAudit>()
                .Property(a => a.ChangeType)
                .HasMaxLength(20);
        }
    }
}
