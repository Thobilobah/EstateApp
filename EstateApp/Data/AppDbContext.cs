using Microsoft.EntityFrameworkCore;
using EstateApp.Entities;

namespace EstateApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<ApartmentInfo> ApartmentInfo { get; set; }
        public DbSet<FeeInfo> FeeInfo { get; set; }
        public DbSet<HouseInfo> HouseInfo { get; set; }
        public DbSet<InvoiceInfo> InvoiceInfo { get; set; }
        public DbSet<PaymentInfo> PaymentInfo { get; set; }
        public DbSet<RolesInfo> RolesInfo { get; set; }
        public DbSet<StreetInfo> StreetInfo { get; set; }
        public DbSet<UserPropertiesInfo> UserPropertiesInfo { get; set; }
        public DbSet<Temp_ViewInfo> Temp_View { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>()
                .Property(u => u.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ApartmentInfo>()
                .Property(a => a.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FeeInfo>()
                .Property(f => f.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<HouseInfo>()
                .Property(h => h.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<InvoiceInfo>()
                .Property(i => i.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PaymentInfo>()
                .Property(p => p.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RolesInfo>()
                .Property(r => r.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<StreetInfo>()
                .Property(s => s.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserPropertiesInfo>()
                .HasKey(up => new { up.userId, up.houseId });

            modelBuilder.Entity<Temp_ViewInfo>(t =>
                {
                    t.HasNoKey();
                    t.ToView("Temp_View");
                });

            // Other configurations
        }
    }
}
