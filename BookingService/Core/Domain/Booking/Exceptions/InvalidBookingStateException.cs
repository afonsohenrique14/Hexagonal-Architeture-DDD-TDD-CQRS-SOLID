using System;

namespace Domain.Booking.Exceptions;

public class InvalidBookingStateException : Exception
{
    public InvalidBookingStateException(string message) : base(message)
    {
    }
}
