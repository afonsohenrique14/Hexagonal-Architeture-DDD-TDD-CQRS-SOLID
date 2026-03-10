using Domain.Entities;
using Domain.Enums;
using Action = Domain.Enums.Action;

namespace DomainTests.Bookings;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    // Positive tests
    [Test]
    public void ShouldAlwaysStartWithCreatedStatus()
    {
        var booking = new Booking();
        Assert.That(booking.CurrentStatus, Is.EqualTo( Status.Created) );
    }

    [Test]
    public void ShouldSetStatusPaidWhenPayingForABookingWithCreatedStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Paid));
    }

    [Test]
    public void ShouldSetStatusCanceledWhenCancelingForABookingWithCreatedStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Cancel);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Canceled));
    }
    [Test]
    public void ShouldSetStatusFinishedWhenFinishingForABookingWithPaidStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay);
        booking.ChangeState(Action.Finish);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Finished));
    }
    [Test]
    public void ShouldSetStatusRefoundedWhenRefoundingForABookingWithPaidStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay);
        booking.ChangeState(Action.Refound);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Refounded));
    }
    [Test]
    public void ShouldSetStatusCreatedWhenReopeningForABookingWithCanceledStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Cancel);
        booking.ChangeState(Action.Reopen);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
    }

    // Negative tests
    [Test]
    public void ShouldNotChangeWhenRefoundingForABookingWithCreatedStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Refound);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
    }
    [Test]
    public void ShouldNotChangeWhenRefoundingForABookingWithFinishedStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay);
        booking.ChangeState(Action.Finish);
        booking.ChangeState(Action.Refound);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Finished));
    }

    [Test]
    public void ShouldNotChangeWhenFinishingForABookingWithCreatedStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Finish);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
    }
    
    [Test]
    public void ShouldNotChangeWhenReopeningForABookingWithCreatedStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Reopen);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
    }
    
    [Test]
    public void ShouldNotChangeWhenCancelingForABookingWithPaidStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay);
        booking.ChangeState(Action.Cancel);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Paid));
    }



}
