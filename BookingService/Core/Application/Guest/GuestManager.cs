
using Application.Guest.DTOs;
using Application.Guest.Mappings;
using Application.Guest.Requests;
using Application.Guest.Responses;
using Application.Ports;
using AutoMapper;
using Domain.Guest.Ports;
namespace Application;

public class GuestManager : IGuestManager
{
    private IGuestRepository _guestRepository;
    private IMapper _mapper;

    public GuestManager(IGuestRepository guestRepository, IMapper mapper)
    {
        _guestRepository = guestRepository;
        _mapper = mapper;
    }
    public async Task<GuestResponse> CreateGuest(CreateGuestRequest request)
    {
        try
        {
            var guest = _mapper.Map<Domain.Guest.Entities.Guest>(request.Data);
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

    public async Task<GuestResponse> GetGuest(int guestId)
    {
        var guest = await _guestRepository.Get(guestId);

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
