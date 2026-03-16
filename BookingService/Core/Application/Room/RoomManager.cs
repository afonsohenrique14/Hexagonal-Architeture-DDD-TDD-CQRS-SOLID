
using Application.Room.DTOs;
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

            return new RoomResponse
            {
                Data = _mapper.Map<ReturnRoomDTO>(room),
                Success = true,
            };

        }
        catch (InvalidRoomDataException)
        {
            return new RoomResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION_ROOM,
                Message = "Missing required information to create a room"
            };
        }
        catch (InvalidRoomPriceException)
        {
            return new RoomResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.INVALID_ROOM_PRICE,
                Message = "The price provided for the room is invalid"
            };
        }
        catch (InvalidRoomLevelException)
            {
                return new RoomResponse
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROOM_LEVEL,
                    Message = "The level provided for the room is invalid"
                };
        }
        catch (Exception)
        {
            return new RoomResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.ROOM_COULD_NOT_STORE_DATA,
                Message = "An error ocurred while creating the room"
            };
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
