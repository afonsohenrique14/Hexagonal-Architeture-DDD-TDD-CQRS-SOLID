using Application;
using Application.MercadoPago;
using Application.Payment.Enums;
using Payment.Application;

namespace Payment.Unit.Tests;

public class PaymentProcessorFactoryTests
{
    [SetUp]
    public void Setup()
    {
    }

    #region  POSITIVE TESTS
    [Test]
    public void Should_Return_Mercado_Pago_Provider()
    {
        var factory = new PaymentProcessorFactory();

        var provider = factory.GetPaymentProcessor(SupportedPaymentProviders.MercadoPago);

        Assert.That(provider, Is.TypeOf<MercadoPagoAdapter>());
    }

    #endregion
    # region NEGATIVE TESTS
    [TestCase(SupportedPaymentProviders.PagSeguro)]
    [TestCase(SupportedPaymentProviders.PayPal)]
    [TestCase(SupportedPaymentProviders.Stripe)]
    public async Task Should_Return_Not_Implemented_Payment_Provider(SupportedPaymentProviders paymentProviders)
    {
        var factory = new PaymentProcessorFactory();

        var provider = factory.GetPaymentProcessor(paymentProviders);

        Assert.That(provider, Is.TypeOf<NotImplementedPaymentProvider>());

        var res = await provider.CapturePayment($"https://www.{paymentProviders}.com/asdf");

        Assert.That(res.Success, Is.False);
        Assert.That(res.ErrorCode, Is.EqualTo(ErrorCodes.PAYMENT_PROVIDER_NOT_IMPLEMENTED));


    }
    #endregion
}
