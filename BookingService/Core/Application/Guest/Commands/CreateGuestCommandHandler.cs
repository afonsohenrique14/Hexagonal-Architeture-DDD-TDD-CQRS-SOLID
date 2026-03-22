using System;
using Application.Guest.DTOs;
using Application.Guest.Mappings;
using Application.Guest.Responses;
using AutoMapper;
using Domain.Guest.Ports;
using MediatR;

namespace Application.Guest.Commands;

public class CreateGuestCommandHandler(IGuestRepository guestRepository, IMapper mapper) : IRequestHandler<CreateGuestCommand, GuestResponse>
{
    private IGuestRepository _guestRepository = guestRepository;
    private IMapper _mapper = mapper;
    public async Task<GuestResponse> Handle(CreateGuestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var guest = _mapper.Map<Domain.Guest.Entities.Guest>(request.createGuestRequest.Data);
            await guest.Save(_guestRepository);

            return ResponseFactory.Ok<GuestResponse>(x =>
            {
                x.Data = _mapper.Map<ReturnGuestDTO>(guest);

            });

        }
        catch (Exception ex)
        {
            var failure = GuestExceptionMapper.Map(ex);

            return ResponseFactory.Fail<GuestResponse>(failure);
        }
    }
}
