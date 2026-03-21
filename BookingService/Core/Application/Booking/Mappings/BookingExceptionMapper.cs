using Domain.Booking.Exceptions;

namespace Application.Booking.Mappings;

public static class BookingExceptionMapper
{
    public static FailureInfo Map(Exception ex)
    {
        return ex switch
        {
            MissingRequiredInformation => new FailureInfo
            {
                ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING,
                Message = "Some required information for creating the booking was not provided"
            },

            InvalidBookingDatesException => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_DATES,
                Message = "The provided booking dates are invalid"
            },

            ConflictingBookingException => new FailureInfo
            {
                ErrorCode = ErrorCodes.CONFLICTING_BOOKING,
                Message = "The provided room is not available for the selected dates"
            },
            Domain.Guest.Exceptions.GuestExceptons => new FailureInfo
            {
                ErrorCode =  ErrorCodes.INVALID_DATA_GUEST,
                Message =  "The provided guest has invalid data"
            },
            Domain.Room.Exceptions.RoomExceptions => new FailureInfo
            {
                ErrorCode =  ErrorCodes.INVALID_DATA_ROOM,
                Message =   "The provided room has invalid data"
            },

            InvalidGuestIDException => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_GUEST_ID,
                Message = "The provided guest has invalid data"
            },

            InvalidRoomIDException => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_ROOM_ID,
                Message = "The provided room does not exist"
            },

            _ => new FailureInfo
            {
                ErrorCode = ErrorCodes.BOOKING_COULD_NOT_BE_CREATED,
                Message = "An unexpected error occurred"
            }
        };
    }
}


