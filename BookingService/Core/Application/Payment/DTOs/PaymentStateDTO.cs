using Application.Payment.Enums;

namespace Application.Payment.DTOs;

public class PaymentStateDTO
{
    public Status Status  { get; set; }

    public string PaymentId { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string Message  { get; set; } = null!;
}
