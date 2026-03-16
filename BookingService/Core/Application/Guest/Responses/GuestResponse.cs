using Application.Guest.DTOs;

namespace Application.Guest.Responses;

public class GuestResponse: Response
{
    public ReturnGuestDTO Data {get; set;}= null!;
}
