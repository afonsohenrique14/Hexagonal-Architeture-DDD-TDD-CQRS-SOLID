using Application.Booking.Requests;
using Application.Booking.Responses;

namespace Application.Booking.Ports;

public interface IBookingManager
{
    Task<BookingResponse> CreateBooking(CreateBookingRequest request);
    Task<BookingResponse> GetBooking(int bookingId);
}
