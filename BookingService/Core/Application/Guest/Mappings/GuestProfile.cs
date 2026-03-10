
using Application.Guest.DTOs;
using AutoMapper;
using Entities = Domain.Entities;

namespace Application.Guest.Mappings;

public class GuestProfile: Profile
{
    public GuestProfile()
    {
        CreateMap<Entities.Guest, GuestDTO>()
            .ForMember(d=> d.IdNumber, opt=> opt.MapFrom(
                 src=> src.DocumentId.IdNumber)
                 )
            .ForMember(d=> d.IdTypeCode, opt=> opt.MapFrom(
                 src=> src.DocumentId.DocumentType)
                 )
            .ReverseMap()
            .ForPath(d=> d.DocumentId.IdNumber, opt => opt.MapFrom(
              src => src.IdNumber  
            ))
            .ForPath(d=> d.DocumentId.DocumentType, opt => opt.MapFrom(
              src => src.IdTypeCode  
            ))           
            ;

    }
}
