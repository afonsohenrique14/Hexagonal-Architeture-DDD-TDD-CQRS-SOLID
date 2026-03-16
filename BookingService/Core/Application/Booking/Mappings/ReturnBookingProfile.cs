
using Application.Booking.DTOs;
using AutoMapper;

namespace Application.Booking.Mappings;

public class ReturnBookingProfile: Profile
{
    public ReturnBookingProfile()
    {
        CreateMap<Domain.Booking.Entities.Booking, ReturnBookingDTO>();
    }
}
