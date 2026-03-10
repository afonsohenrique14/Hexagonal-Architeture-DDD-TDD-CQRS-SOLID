using Application.Guest.DTOs;

namespace Application.Guest.Responses;

public class GuestResponse: Response
{
    public GuestDTO Data {get; set;}= null!;
}
