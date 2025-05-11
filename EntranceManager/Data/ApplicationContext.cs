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
               .WithMany(u => u.OwnedApartments)
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

            modelBuilder.Entity<ApartmentUser>()
                .HasKey(au => new { au.UserId, au.ApartmentId });

            modelBuilder.Entity<ApartmentUser>()
                .HasOne(au => au.User)
                .WithMany(u => u.ApartmentUsers)
                .HasForeignKey(au => au.UserId);

            modelBuilder.Entity<ApartmentUser>()
                .HasOne(au => au.Apartment)
                .WithMany(a => a.ApartmentUsers)
                .HasForeignKey(au => au.ApartmentId);

            modelBuilder.Entity<EntranceUser>()
                .HasKey(eu => new { eu.UserId, eu.EntranceId });

            modelBuilder.Entity<EntranceUser>()
                .HasOne(eu => eu.User)
                .WithMany(u => u.EntranceUsers)
                .HasForeignKey(eu => eu.UserId);

            modelBuilder.Entity<EntranceUser>()
                .HasOne(eu => eu.Entrance)
                .WithMany(e => e.EntranceUsers)
                .HasForeignKey(eu => eu.EntranceId);
        }
    }
}
