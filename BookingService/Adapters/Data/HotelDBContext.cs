
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Entities = Domain.Entities;
namespace Data
{
    public class HotelDBContext(DbContextOptions<HotelDBContext> options) : DbContext(options)
    {
        public virtual DbSet<Entities.Guest> Guests { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GuestConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
        }

    }
}
