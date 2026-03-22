using Application.Room.DTOs;
using Application.Room.Responses;
using AutoMapper;
using Domain.Room.Ports;
using MediatR;

namespace Application.Room.Queries;

public class GetRoomQueryHandler(IRoomRepository roomRepository, IMapper mapper) : IRequestHandler<GetRoomQuery, RoomResponse>
{

    private IRoomRepository _roomRepository = roomRepository;
    private IMapper _mapper = mapper;
    public async Task<RoomResponse> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.Get(request.roomId);

        if (room == null)
        {
            return new RoomResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.NOT_FOUND,
                Message = "The room with the provided id was not found"
            };
        }

        return new RoomResponse
        {
            Success = true,
            Data = _mapper.Map<ReturnRoomDTO>(room)
        };
    }
}
