

using Microsoft.EntityFrameworkCore;
using Entities_Guest = Domain.Guest.Entities;
using Entities_Room = Domain.Room.Entities;
using Entities_Booking = Domain.Booking.Entities;
using Data.Booking;
namespace Data
{
    public class HotelDBContext(DbContextOptions<HotelDBContext> options) : DbContext(options)
    {
        public virtual DbSet<Entities_Guest.Guest> Guests { get; set; }
        public virtual DbSet<Entities_Room.Room> Rooms { get; set; }
        public virtual DbSet<Entities_Booking.Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GuestConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfigurantion());
        }

    }
}
