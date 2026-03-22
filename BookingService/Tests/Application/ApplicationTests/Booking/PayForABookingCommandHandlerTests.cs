using Application;
using Application.Booking.Commands;
using Application.Payment.DTOs;
using Application.Payment.Enums;
using Moq;

namespace ApplicationTests.Booking;

public class PayForABookingCommandHandlerTests : BookingTestFixtureBase
{
    [Test]
    public async Task Pay_Booking_Should_Return_Success_And_PaymentId()
    {
        var handler = new PayForABookingCommandHandler(
            BookingRepo.Object,
            GuestRepo.Object,
            RoomRepo.Object,
            PaymentProcessorFactory.Object);

        var dto = new PaymentRequestDTO
        {
            BookingId = CreatedBookingId,
            SelectedPaymentProvider = SupportedPaymentProviders.MercadoPago,
            PaymentIntention = "https://www.mercadopago.com.br/asdf",
            SelectedPaymentMethod = SupportedPaymentMethods.CreditCard
        };

        var result = await handler.Handle(new PayForABookingCommand { paymentRequestDTO = dto }, default);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data.PaymentId, Is.EqualTo("123"));
    }

    [Test]
    public async Task Pay_Booking_Should_Return_Invalid_Booking_Id_When_Booking_Not_Found()
    {
        var handler = new PayForABookingCommandHandler(
            BookingRepo.Object,
            GuestRepo.Object,
            RoomRepo.Object,
            PaymentProcessorFactory.Object);

        var dto = new PaymentRequestDTO
        {
            BookingId = 5,
            SelectedPaymentProvider = SupportedPaymentProviders.MercadoPago,
            PaymentIntention = "https://www.mercadopago.com.br/asdf",
            SelectedPaymentMethod = SupportedPaymentMethods.CreditCard
        };

        var result = await handler.Handle(new PayForABookingCommand { paymentRequestDTO = dto }, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.INVALID_BOOKING_ID));
    }
    [Test]
    public async Task Pay_Booking_Should_Return_Duplicate_Payment()
    {

        
        var bookingId = 111;

        var booking = new Domain.Booking.Entities.Booking
            {
                Id = bookingId,
                RoomId = 1,
                GuestId = 1,
                Start = DateTime.UtcNow.AddDays(-2),
                End = DateTime.UtcNow.AddDays(-1),
            };

        booking.ChangeState(Domain.Booking.Enums.Action.Pay);


        BookingRepo
            .Setup(x => x.Get(bookingId))
            .ReturnsAsync(booking);


        var handler = new PayForABookingCommandHandler(
            BookingRepo.Object,
            GuestRepo.Object,
            RoomRepo.Object,
            PaymentProcessorFactory.Object);

       
        var dto = new PaymentRequestDTO
        {
            BookingId = bookingId,
            SelectedPaymentProvider = SupportedPaymentProviders.MercadoPago,
            PaymentIntention = "https://www.mercadopago.com.br/asdf",
            SelectedPaymentMethod = SupportedPaymentMethods.CreditCard
        };

         // primeiro pagamento
        var result = await handler.Handle(new PayForABookingCommand { paymentRequestDTO = dto }, default);
        

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.DUPLICATED_PAYMENT));
    }
}