using Application;
using Application.Booking.Queries;

namespace ApplicationTests.Booking;

public class Get_BookingQueryHandlerTests : BookingTestFixtureBase
{
    [Test]
    public async Task Get_Booking_Should_Return_Booking_With_Room_And_Guest()
    {
        var handler = new GetBookingQueryHandler(BookingRepo.Object, Mapper);

        var result = await handler.Handle(new GetBookingQuery { bookingId = CreatedBookingId }, default);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data.Id, Is.EqualTo(CreatedBookingId));
        Assert.That(result.Data.Room, Is.Not.Null);
        Assert.That(result.Data.Guest, Is.Not.Null);
    }

    [Test]
    public async Task Get_Booking_Should_Return_NotFound_When_Id_Does_Not_Exist()
    {
        var handler = new GetBookingQueryHandler(BookingRepo.Object, Mapper);

        var result = await handler.Handle(new GetBookingQuery { bookingId = 999999 }, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.NOT_FOUND));
    }
}