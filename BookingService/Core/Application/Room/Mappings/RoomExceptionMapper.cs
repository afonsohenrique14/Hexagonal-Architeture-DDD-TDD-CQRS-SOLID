using System;
using Domain.Room.Exceptions;

namespace Application.Room.Mappings;

public static class RoomExceptionMapper
{
    public static FailureInfo Map(Exception ex)
    {
        return ex switch
        {
            InvalidRoomDataException => new FailureInfo
            {
                ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION_ROOM,
                Message = "Missing required information to create a room"
            },
            InvalidRoomPriceException => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_ROOM_PRICE,
                Message = "The price provided for the room is invalid"
            },
            InvalidRoomLevelException => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_ROOM_LEVEL,
                Message = "The level provided for the room is invalid"
            },
            _ => new FailureInfo
            {
                ErrorCode = ErrorCodes.ROOM_COULD_NOT_STORE_DATA,
                Message = "An error ocurred while creating the room"
            }
        };
    }
}
