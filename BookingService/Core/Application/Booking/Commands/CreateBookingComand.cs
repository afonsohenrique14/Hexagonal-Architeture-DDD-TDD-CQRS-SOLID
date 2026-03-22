
using Application.Booking.Requests;
using Application.Booking.Responses;
using MediatR;

namespace Application.Booking.Commands;

public class CreateBookingComand: IRequest<BookingResponse>
{
    public CreateBookingRequest createBookingRequest {get; set;}= null!;


}
