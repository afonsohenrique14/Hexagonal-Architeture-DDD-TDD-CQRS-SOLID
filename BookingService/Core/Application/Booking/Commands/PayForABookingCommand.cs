using Application.Payment.DTOs;
using Application.Payment.Responses;
using MediatR;

namespace Application.Booking.Commands;

public class PayForABookingCommand: IRequest<PaymentResponse>
{
    public PaymentRequestDTO paymentRequestDTO {get; set;} = null!;
}
