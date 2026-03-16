
using Domain.Room.Ports;
using Microsoft.EntityFrameworkCore;
using Entities = Domain.Room.Entities;

namespace Data.Room;

public class RoomRepostory(HotelDBContext hotelDBContext) : IRoomRepository
{
    private HotelDBContext _hotelDbContext = hotelDBContext;

    public async Task<int> Create(Entities.Room room)
    {
        await _hotelDbContext.Rooms.AddAsync(room);
        await _hotelDbContext.SaveChangesAsync();
        return room.Id;
    }

    public async Task<Entities.Room?> Get(int id)
    {
        return await _hotelDbContext.Rooms
            .Include(r=> r.Bookings)
            .Where(r=> r.Id == r.Id).FirstAsync();
    }
}
