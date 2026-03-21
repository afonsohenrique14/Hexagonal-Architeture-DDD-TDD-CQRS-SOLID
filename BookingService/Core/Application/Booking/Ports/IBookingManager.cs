using Application.Booking.Requests;
using Application.Booking.Responses;
using Application.Payment.DTOs;
using Application.Payment.Responses;

namespace Application.Booking.Ports;

public interface IBookingManager
{
    Task<BookingResponse> CreateBooking(CreateBookingRequest request);
    Task<BookingResponse> GetBooking(int bookingId);
    Task<PaymentResponse> PayForABooking(PaymentRequestDTO data);
}
