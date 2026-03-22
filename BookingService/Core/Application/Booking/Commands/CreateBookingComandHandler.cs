using Application.Booking.Ports;
using Application.Booking.Responses;
using MediatR;

namespace Application.Booking.Commands;

public class CreateBookingComandHandler : IRequestHandler<CreateBookingComand, BookingResponse>
{
    private IBookingManager _bookingManager;

    public CreateBookingComandHandler(IBookingManager bookingManager)
    {
        _bookingManager = bookingManager;
    }
    public Task<BookingResponse> Handle(CreateBookingComand request, CancellationToken cancellationToken)
    {
        return _bookingManager.CreateBooking(request.createBookingRequest);
    }
}
