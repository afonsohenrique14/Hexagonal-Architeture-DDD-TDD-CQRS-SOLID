using Application.Booking.Mappings;
using Application.Payment.Ports;
using Application.Payment.Responses;
using Domain.Booking.Ports;
using Domain.Guest.Ports;
using Domain.Room.Ports;
using MediatR;

namespace Application.Booking.Commands;

public class PayForABookingCommandHandler(
    IBookingRepository bookingRepository, 
    IGuestRepository guestRepository, 
    IRoomRepository roomRepository, 
    IPaymentProcessorFactory paymentProcessorFactory
    ) : IRequestHandler<PayForABookingCommand, PaymentResponse>
{

    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IGuestRepository _guestRepository = guestRepository;
    private readonly IRoomRepository _roomRepository = roomRepository;


     private readonly IPaymentProcessorFactory _paymentProcessorFactory = paymentProcessorFactory;

    public async Task<PaymentResponse> Handle(PayForABookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var booking = await _bookingRepository.Get(request.paymentRequestDTO.BookingId);

            if (booking == null)
            {
                return new PaymentResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_BOOKING_ID,
                    Message = "No booking found with the provided id"
                };
            }

            var paymentProcessor = _paymentProcessorFactory.GetPaymentProcessor(request.paymentRequestDTO.SelectedPaymentProvider);

            var response = await paymentProcessor.CapturePayment(request.paymentRequestDTO.PaymentIntention);

            if (!response.Success)
            {
                return response;
            }

            booking.ChangeState(Domain.Booking.Enums.Action.Pay);

            await booking.Save(_bookingRepository, _guestRepository, _roomRepository);

            return ResponseFactory.Ok<PaymentResponse>(r =>
                {
                    r.Data = response.Data;
                    r.Message = "Payment successfully processed";
                }
            );
        }
        catch (Exception ex)
        {
            var failure = BookingExceptionMapper.Map(ex);

            return ResponseFactory.Fail<PaymentResponse>(failure);
        }
    }
}
