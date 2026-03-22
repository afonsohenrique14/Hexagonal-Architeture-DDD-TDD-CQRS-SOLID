using Application.Room.Requests;
using Application.Room.Responses;
using MediatR;

namespace Application.Room.Commands;

public class CreateRoomCommand: IRequest<RoomResponse>
{
    public CreateRoomRequest createRoomRequest {get; set;} = null!;
}
