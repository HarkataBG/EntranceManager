using EntranceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EntranceManager.Data
{

    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Entrance> Entrances { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<ApartmentFee> ApartmentFees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Fee>()
                        .Property(f => f.Amount)
                        .HasPrecision(18, 2);

            modelBuilder.Entity<Apartment>()
               .HasOne(a => a.OwnerUser)
               .WithMany(u => u.Apartments)
               .HasForeignKey(a => a.OwnerUserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApartmentFee>()
                .HasOne(af => af.Fee)
                .WithMany(f => f.ApartmentFees)
                .HasForeignKey(af => af.FeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApartmentFee>()
                .HasOne(af => af.Apartment)
                .WithMany(a => a.ApartmentFees)
                .HasForeignKey(af => af.ApartmentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
