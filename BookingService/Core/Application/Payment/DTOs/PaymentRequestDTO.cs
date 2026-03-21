using System;
using Application.Payment.Enums;

namespace Application.Payment.DTOs;

public class PaymentRequestDTO
{
    public int BookingId { get; set; }

    public string PaymentIntention { get; set; } = null!;

    public SupportedPaymentProviders SelectedPaymentProvider { get; set; }
    public SupportedPaymentMethods SelectedPaymentMethod { get; set; }

}
