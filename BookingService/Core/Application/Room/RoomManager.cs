
using Application.Room.DTOs;
using Application.Room.Mappings;
using Application.Room.Ports;
using Application.Room.Requests;
using Application.Room.Responses;
using AutoMapper;
using Domain.Room.Exceptions;
using Domain.Room.Ports;
using Entities = Domain.Room.Entities;

namespace Application.Room;

public class RoomManager : IRoomManager
{

    private IRoomRepository _roomRepository;
    private IMapper _mapper;

    public RoomManager(IRoomRepository roomRepository, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _mapper = mapper;
    }

    public async Task<RoomResponse> CreateRoom(CreateRoomRequest request)
    {
        try
        {
            var room = _mapper.Map<Entities.Room>(request.Data);
            await room.Save(_roomRepository);

            return ResponseFactory.Ok<RoomResponse>(
                x =>
                {
                    x.Data = _mapper.Map<ReturnRoomDTO>(room);
                }
            );

        }
        catch (Exception ex)
        {
            var failure = RoomExceptionMapper.Map(ex);

            return ResponseFactory.Fail<RoomResponse>(failure);
        }
    }

    public async Task<RoomResponse> GetRoom(int roomId)
    {
        var room = await _roomRepository.Get(roomId);

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
