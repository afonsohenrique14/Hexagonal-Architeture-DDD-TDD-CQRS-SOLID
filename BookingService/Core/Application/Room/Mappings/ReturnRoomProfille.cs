
using Application.Room.DTOs;
using AutoMapper;
using Entities = Domain.Room.Entities;
namespace Application.Room.Mappings;

public class ReturnRoomProfille: Profile

{
    public ReturnRoomProfille()
    {
        CreateMap<Entities.Room, ReturnRoomDTO>()
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
