
using Application.Room.DTOs;

namespace Application.Room.Responses;

public class RoomResponse: Response
{
    public ReturnRoomDTO Data {get; set;}= null!;
}
