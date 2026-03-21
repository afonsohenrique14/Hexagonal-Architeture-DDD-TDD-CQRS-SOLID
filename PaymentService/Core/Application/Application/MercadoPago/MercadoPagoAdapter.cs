
using Application.MercadoPago.Exceptions;
using Application.Payment;
using Application.Payment.DTOs;
using Application.Payment.Responses;

namespace Application.MercadoPago;

public class MercadoPagoAdapter : IPaymentProcessor
{

    public Task<PaymentResponse> CapturePayment(string paymentIntention)
    {

        try
        {
            if (string.IsNullOrWhiteSpace(paymentIntention))
            {
                throw new InvalidaPaymentIntentionException();
            }

            paymentIntention += "/success";

            var dto = new PaymentStateDTO
            {
                CreatedDate = DateTime.Now,
                Message = $"Successfully paid {paymentIntention}",
                PaymentId = "123",
                Status = Payment.Enums.Status.Success
            };

            
            var resp = new PaymentResponse
            {
                Success = true,
                Data = dto,
                Message = "Payment sucessfully processed"

            };

            return Task.FromResult(resp);
        }
        catch (InvalidaPaymentIntentionException)
        {

            var resp = new PaymentResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.INVALID_PAYMENT_INTENTION,
            };

            return Task.FromResult(resp);
        }

    }


}
