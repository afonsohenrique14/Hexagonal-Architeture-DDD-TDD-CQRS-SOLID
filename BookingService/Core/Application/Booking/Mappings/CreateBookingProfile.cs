
using Application.Booking.DTOs;
using AutoMapper;

namespace Application.Booking.Mappings;

public class CreateBookingProfile: Profile
{
    public CreateBookingProfile()
    {
        CreateMap<CreateBookingDTO, Domain.Booking.Entities.Booking>();
        
    }
}
