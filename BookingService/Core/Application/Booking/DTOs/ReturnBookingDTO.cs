using System;
using Application.Guest.DTOs;
using Application.Room.DTOs;

namespace Application.Booking.DTOs;

public class ReturnBookingDTO
{
    public int Id { get; set; }
    public DateTime PlacedAt { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public ReturnRoomDTO Room { get; set; } = null!;
    public ReturnGuestDTO Guest { get; set; } = null!;
}
