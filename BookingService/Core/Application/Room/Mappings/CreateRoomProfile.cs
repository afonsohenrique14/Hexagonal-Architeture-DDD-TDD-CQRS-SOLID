using Application.Room.DTOs;
using Entities = Domain.Room.Entities;
using AutoMapper;

namespace Application.Room.Mappings;

public class CreateRoomProfile: Profile
{
    public CreateRoomProfile()
    {
        CreateMap<Entities.Room, CreateRoomDTO>()
            .ForMember(d => d.Currency, opt=> opt.MapFrom(
                src => src.Price.Currency
            ))
            .ForMember(d => d.Value, opt=> opt.MapFrom(
                src => src.Price.Value
            ))
            .ReverseMap()
                .ForPath(d => d.Price.Currency, opt=> opt.MapFrom(
                    src => src.Currency
                ))
                .ForPath(d => d.Price.Value, opt=> opt.MapFrom(
                    src => src.Value
                ));

        
    }
}
