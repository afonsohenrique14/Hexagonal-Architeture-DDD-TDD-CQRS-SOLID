namespace Domain.Booking.Ports;

public interface IBookingRepository
{
    Task<Entities.Booking?> Get(int id);

    Task<int> Create(Entities.Booking booking);

    Task<bool> ExistsActiveBookingForRoom(int bookingId, int roomId, DateTime start, DateTime end);

    Task Update(Entities.Booking booking);
}
