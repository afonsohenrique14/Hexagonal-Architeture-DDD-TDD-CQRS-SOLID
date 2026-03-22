using System;
using Application.Room.Responses;
using MediatR;

namespace Application.Room.Queries;

public class GetRoomQuery: IRequest<RoomResponse>
{
    public int roomId { get; set; }
}
