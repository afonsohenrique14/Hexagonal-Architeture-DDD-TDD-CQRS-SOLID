using System;
using Application.Booking.DTOs;

namespace Application.Booking.Responses;

public class BookingResponse: Response
{
    public ReturnBookingDTO Data {get; set;}= null!;
}
