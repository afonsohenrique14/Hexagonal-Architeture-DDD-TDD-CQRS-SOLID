using System;

namespace Application.Booking.DTOs;

public class CreateBookingDTO
{

    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
}
