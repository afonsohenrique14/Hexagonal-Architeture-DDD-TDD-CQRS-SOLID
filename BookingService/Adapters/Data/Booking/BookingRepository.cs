
using Domain.Booking.Ports;
using Microsoft.EntityFrameworkCore;

namespace Data.Booking;

public class BookingRepository : IBookingRepository
{
    private HotelDBContext _hotelDbContext;
    public BookingRepository(HotelDBContext hotelDBContext)
    {
        _hotelDbContext = hotelDBContext;
    }
    public async Task<int> Create(Domain.Booking.Entities.Booking booking)
    {
        await _hotelDbContext.Bookings.AddAsync(booking);
        await _hotelDbContext.SaveChangesAsync();
        return booking.Id;
    }

    public async Task<bool> ExistsActiveBookingForRoom(int roomId, DateTime start, DateTime end)
    {
        return await _hotelDbContext.Bookings.AnyAsync(
            b=> b.RoomId == roomId &&
            (
                b.End > start &&
                b.Start < end 
            ) 
        );
    }

    public async Task<Domain.Booking.Entities.Booking?> Get(int id)
    {
        return await _hotelDbContext.Bookings
        .Include(b=> b.Guest)
        .Include(b=> b.Room)
        .FirstOrDefaultAsync(b=> b.Id == id);
    }
}
