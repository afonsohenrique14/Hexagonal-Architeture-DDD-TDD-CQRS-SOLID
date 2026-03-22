using Application.Guest.DTOs;
using Application.Guest.Responses;
using AutoMapper;
using Domain.Guest.Ports;
using MediatR;

namespace Application.Guest.Queries;

public class GetGuestQueryHandler(IGuestRepository guestRepository, IMapper mapper) : IRequestHandler<GetGuestQuery, GuestResponse>
{
    private IGuestRepository _guestRepository = guestRepository;
    private IMapper _mapper = mapper;
    public async Task<GuestResponse> Handle(GetGuestQuery request, CancellationToken cancellationToken)
    {
       var guest = await _guestRepository.Get(request.GuestId);

        if(guest == null)
        {
            return new GuestResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.NOT_FOUND,
                Message = "Guest not found"
            };
        }

        return ResponseFactory.Ok<GuestResponse>(x =>
        {
            x.Data = _mapper.Map<ReturnGuestDTO>(guest);

        });
    }
}
