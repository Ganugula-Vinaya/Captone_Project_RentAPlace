using Microsoft.EntityFrameworkCore;
using RentAPlace.Api.Models;

namespace RentAPlace.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users => Set<User>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for join table
            modelBuilder.Entity<PropertyAmenity>().HasKey(pa => new { pa.PropertyId, pa.AmenityId });

            // One-to-many property -> images
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Property!)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
