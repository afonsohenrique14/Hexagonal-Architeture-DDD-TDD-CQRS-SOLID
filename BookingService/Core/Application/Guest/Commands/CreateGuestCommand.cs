using Application.Guest.Requests;
using Application.Guest.Responses;
using MediatR;

namespace Application.Guest.Commands;

public class CreateGuestCommand: IRequest<GuestResponse>
{
    public CreateGuestRequest createGuestRequest { get; set; } = null!;
}
