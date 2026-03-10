using Application.Guest.DTOs;

namespace Application.Guest.Requests;

public class CreateGuestRequest
{
     public GuestDTO Data {get; set;}= null!;
}
