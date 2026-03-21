using Application.Payment.DTOs;

namespace Application.Payment.Responses;

public class PaymentResponse: Response
{
    public PaymentStateDTO Data {get; set;}= null!;
}
