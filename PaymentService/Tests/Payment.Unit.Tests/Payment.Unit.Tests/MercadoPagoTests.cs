using Application;
using Application.MercadoPago;
using Application.Payment.Enums;
using Payment.Application;

namespace Payment.Unit.Tests;

public class MercadoPagoTests
{
    [SetUp]
    public void Setup()
    {
    }

    #region POSITIVE TESTS
    [Test]
    public void Should_Return_Mercado_Pago_Adapter_Provider()
    {
        var factory = new PaymentProcessorFactory();

        var provider = factory.GetPaymentProcessor(SupportedPaymentProviders.MercadoPago);

        Assert.That(provider, Is.TypeOf<MercadoPagoAdapter>());
    }
    [Test]
    public async Task ShoudSucessfullyProcessPaymentAsync()
    {
        var factory = new PaymentProcessorFactory();

        var provider = factory.GetPaymentProcessor(SupportedPaymentProviders.MercadoPago);

        var res = await provider.CapturePayment($"https://www.mercadopago.com/asdf");

        Assert.That(res.Success, Is.True);
        Assert.That(res.Data, Is.Not.Null);
        Assert.That(res.Data.CreatedDate, Is.Not.EqualTo(default(DateTime)));
        Assert.That(res.Data.PaymentId, Is.Not.Null);

    } 

    #endregion
    #region NEGATIVE TESTS
    [Test]
    public async Task Should_Fail_When_Payment_Intention_String_Is_InvalidAsync()
    {
        var factory = new PaymentProcessorFactory();

        var provider = factory.GetPaymentProcessor(SupportedPaymentProviders.MercadoPago);

        var res = await provider.CapturePayment("");

        Assert.That(res.Success, Is.False);
        Assert.That(res.ErrorCode, Is.EqualTo(ErrorCodes.INVALID_PAYMENT_INTENTION));


        
    }
    #endregion

}
