using Application;
using Application.Booking.Commands;
using Application.Booking.DTOs;
using Application.Booking.Requests;

namespace ApplicationTests.Booking;

public class CreateBookingComandHandlerTests : BookingTestFixtureBase
{
    [Test]
    public async Task Create_Booking_Should_Return_Created_Booking()
    {
        var handler = new CreateBookingComandHandler(
            BookingRepo.Object, GuestRepo.Object, RoomRepo.Object, Mapper);

        var dto = new CreateBookingDTO
        {
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(3),
            RoomId = 1,
            GuestId = 1
        };

        var command = new CreateBookingComand
        {
            createBookingRequest = new CreateBookingRequest { Data = dto }
        };

        var result = await handler.Handle(command, default);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data.Id, Is.EqualTo(CreatedBookingId));
    }

    [TestCase("2024-01-10", "2024-01-05", 1, 1, ErrorCodes.INVALID_DATES)]
    [TestCase("2024-01-10", "2024-01-10", 1, 1, ErrorCodes.INVALID_DATES)]
    [TestCase("2024-01-10", "2024-01-15", 995, 1, ErrorCodes.INVALID_ROOM_ID)]
    [TestCase("2024-01-10", "2024-01-15", 1, 995, ErrorCodes.INVALID_GUEST_ID)]
    [TestCase("2024-01-10", "2024-01-15", 0, 1, ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING)]
    [TestCase("2024-01-10", "2024-01-15", 1, 0, ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING)]
    [TestCase("2024-01-10", "2024-01-15", 121, 1, ErrorCodes.INVALID_DATA_ROOM)]
    [TestCase("2024-01-10", "2024-01-15", 999, 1, ErrorCodes.INVALID_DATA_ROOM)]
    [TestCase("2024-01-10", "2024-01-15", 1, 999, ErrorCodes.INVALID_DATA_GUEST)]
    public async Task Create_Booking_Should_Return_Error_For_Invalid_Data(
        DateTime start, DateTime end, int roomId, int guestId, ErrorCodes expected)
    {
        var handler = new CreateBookingComandHandler(
            BookingRepo.Object, GuestRepo.Object, RoomRepo.Object, Mapper);

        var dto = new CreateBookingDTO
        {
            Start = start,
            End = end,
            RoomId = roomId,
            GuestId = guestId
        };

        var command = new CreateBookingComand
        {
            createBookingRequest = new CreateBookingRequest { Data = dto }
        };

        var result = await handler.Handle(command, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(expected));
    }

    [TestCase("2024-01-10", "2024-01-15")]
    [TestCase("2024-01-09", "2024-01-11")]
    [TestCase("2024-01-14", "2024-01-16")]
    [TestCase("2024-01-09", "2024-01-16")]
    [TestCase("2024-01-08", "2024-01-16")]
    public async Task Create_Booking_Should_Return_Conflict_When_Second_Booking_Overlaps(DateTime start, DateTime end)
    {
        var handler = new CreateBookingComandHandler(
            BookingRepo.Object, GuestRepo.Object, RoomRepo.Object, Mapper);

        var first = new CreateBookingComand
        {
            createBookingRequest = new CreateBookingRequest
            {
                Data = new CreateBookingDTO
                {
                    Start = new DateTime(2024, 1, 10),
                    End = new DateTime(2024, 1, 15),
                    RoomId = 1,
                    GuestId = 1
                }
            }
        };

        var firstRes = await handler.Handle(first, default);
        Assert.That(firstRes.Success, Is.True);

        var second = new CreateBookingComand
        {
            createBookingRequest = new CreateBookingRequest
            {
                Data = new CreateBookingDTO
                {
                    Start = start,
                    End = end,
                    RoomId = 1,
                    GuestId = 1
                }
            }
        };

        var secondRes = await handler.Handle(second, default);

        Assert.That(secondRes.Success, Is.False);
        Assert.That(secondRes.ErrorCode, Is.EqualTo(ErrorCodes.CONFLICTING_BOOKING));
        Assert.That(Store.Count, Is.EqualTo(1));
    }
}