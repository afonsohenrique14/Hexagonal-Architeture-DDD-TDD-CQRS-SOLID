using Domain.Guest.Ports;
using Entities = Domain.Guest.Entities;

namespace Data.Guest;

public class GuestRepository : IGuestRepository
{
    private HotelDBContext _hotelDbContext;
    public GuestRepository(HotelDBContext hotelDBContext)
    {
        _hotelDbContext = hotelDBContext;
    }
    public async Task<int> Create(Entities.Guest guest)
    {
        await _hotelDbContext.Guests.AddAsync(guest);
        await _hotelDbContext.SaveChangesAsync();
        return guest.Id;
    }

    public async Task<Entities.Guest?> Get(int id)
    {
        return await _hotelDbContext.Guests.FindAsync(id);
    }
}
