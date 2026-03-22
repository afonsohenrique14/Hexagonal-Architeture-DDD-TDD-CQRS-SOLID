using Application.Booking.DTOs;

namespace Application.Booking.Requests;

public class CreateBookingRequest
{
    public CreateBookingDTO   Data {get; set;}= null!;
}
