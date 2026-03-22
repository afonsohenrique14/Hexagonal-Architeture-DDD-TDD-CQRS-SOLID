using Domain.Booking.Entities;
using Domain.Booking.Enums;
using Domain.Booking.Exceptions;
using Action = Domain.Booking.Enums.Action;
namespace DomainTests.Bookings;

public class BookingStateTests
{
    // -------------------------
    // Initial state
    // -------------------------
    [Test]
    public void Should_Always_Start_With_Created_Status()
    {
        var booking = new Booking();
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
    }

    // -------------------------
    // Valid transitions
    // -------------------------
    [TestCase(Action.Pay,     Status.Paid)]
    [TestCase(Action.Cancel,  Status.Canceled)]
    public void Should_Change_Status_From_Created_When_Action_Is_Valid(
        Action action,
        Status expectedStatus)
    {
        var booking = new Booking();

        booking.ChangeState(action);

        Assert.That(booking.CurrentStatus, Is.EqualTo(expectedStatus));
    }

    [TestCase(Action.Finish,  Status.Finished)]
    [TestCase(Action.Refound, Status.Refounded)]
    public void Should_Change_Status_From_Paid_When_Action_Is_Valid(
        Action action,
        Status expectedStatus)
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay); // precondition

        booking.ChangeState(action);

        Assert.That(booking.CurrentStatus, Is.EqualTo(expectedStatus));
    }

    [Test]
    public void Should_Reopen_Canceled_Booking_To_Created()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Cancel);

        booking.ChangeState(Action.Reopen);

        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
    }

    // -------------------------
    // Invalid transitions
    // -------------------------
    [TestCase(Status.Created,  Action.Refound)]
    [TestCase(Status.Created,  Action.Finish)]
    [TestCase(Status.Created,  Action.Reopen)]
    [TestCase(Status.Paid,     Action.Cancel)]
    [TestCase(Status.Finished, Action.Refound)]
    public void Should_Throw_When_Invalid_Action_Is_Applied(
        Status initialStatus,
        Action invalidAction)
    {
        var booking = new Booking();

        // Arrange initial state
        booking = initialStatus switch
        {
            Status.Paid => Apply(booking, Action.Pay),
            Status.Finished => Apply(Apply(booking, Action.Pay), Action.Finish),
            Status.Created => booking,
            _ => throw new NotSupportedException()
        };

        // Act + Assert
        Assert.Throws<InvalidBookingStateException>(
            () => booking.ChangeState(invalidAction));

        // Status must remain unchanged
        Assert.That(booking.CurrentStatus, Is.EqualTo(initialStatus));
    }

    // -------------------------
    // Helper
    // -------------------------
    private static Booking Apply(Booking booking, Action action)
    {
        booking.ChangeState(action);
        return booking;
    }
}