using Domain.Ports;

namespace Data.Guest;

public class GuestRepository : IGuestRepository
{
    private HotelDBContext _hotelDbContext;
    public GuestRepository(HotelDBContext hotelDBContext)
    {
        _hotelDbContext = hotelDBContext;
    }
    public async Task<int> Create(Domain.Entities.Guest guest)
    {
        await _hotelDbContext.Guests.AddAsync(guest);
        await _hotelDbContext.SaveChangesAsync();
        return guest.Id;
    }

    public async Task<Domain.Entities.Guest?> Get(int id)
    {
        return await _hotelDbContext.Guests.FindAsync(id);
    }
}
