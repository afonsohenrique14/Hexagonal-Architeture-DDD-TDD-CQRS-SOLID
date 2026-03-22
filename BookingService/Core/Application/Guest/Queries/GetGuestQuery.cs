using System;
using Application.Guest.Responses;
using MediatR;

namespace Application.Guest.Queries;

public class GetGuestQuery: IRequest<GuestResponse>
{
    public int GuestId {get; set;}
}
