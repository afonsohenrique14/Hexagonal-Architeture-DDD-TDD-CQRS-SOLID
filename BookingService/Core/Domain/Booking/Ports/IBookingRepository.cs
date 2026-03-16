using System;

namespace Domain.Booking.Ports;

public interface IBookingRepository
{
    Task<Entities.Booking?> Get(int id);

    Task<int> Create(Entities.Booking booking);

    Task<bool> ExistsActiveBookingForRoom(int roomId, DateTime start, DateTime end);
}
