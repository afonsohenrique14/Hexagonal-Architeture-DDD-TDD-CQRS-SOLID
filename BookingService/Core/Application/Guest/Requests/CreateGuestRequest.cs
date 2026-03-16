using Application.Guest.DTOs;

namespace Application.Guest.Requests;

public class CreateGuestRequest
{
     public CreateGuestDTO Data {get; set;}= null!;
}
