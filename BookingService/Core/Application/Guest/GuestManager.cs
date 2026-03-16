
using Application.Guest.DTOs;
using Application.Guest.Requests;
using Application.Guest.Responses;
using Application.Ports;
using AutoMapper;
using Domain.Guest.Exceptions;
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

            return new GuestResponse
            {
                Data = _mapper.Map<ReturnGuestDTO>(guest),
                Success = true,

            };

        }
        catch (InvalidPersonDucumentIdException)
        {
            return new GuestResponse
            {
                
                Success = false,
                ErrorCode = ErrorCodes.INVALID_PERSON_DOCUMENT,
                Message = "The document id or document type provided is invalid"
            };
        }
        catch (MissingRequiredInformation)
        {
            return new GuestResponse
            {
                
                Success = false,
                ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION,
                Message = "Missing required information"
            };
        }
        catch (InvalidEmailException)
        {
            return new GuestResponse
            {
                
                Success = false,
                ErrorCode = ErrorCodes.INVALID_EMAIL,
                Message = "The email provided is invalid"
            };
        }
        catch (Exception)
        {
            return new GuestResponse
            {
                
                Success = false,
                ErrorCode = ErrorCodes.COULD_NOT_STORE_DATA,
                Message = "There was an erro when saving to DB"
            };
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

        return new GuestResponse
        {
            Data = _mapper.Map<ReturnGuestDTO>(guest),
            Success = true
        };


    }
}
