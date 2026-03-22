using Entities = Domain.Room.Entities;
using Application.Room.Responses;
using AutoMapper;
using Domain.Room.Ports;
using MediatR;
using Application.Room.DTOs;
using Application.Room.Mappings;

namespace Application.Room.Commands;

public class CreateRoomCommandHandler(IRoomRepository roomRepository, IMapper mapper) : IRequestHandler<CreateRoomCommand, RoomResponse>
{
    private IRoomRepository _roomRepository = roomRepository;
    private IMapper _mapper = mapper;
    public async Task<RoomResponse> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
       try
        {
            var room = _mapper.Map<Entities.Room>(request.createRoomRequest.Data);
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
}
