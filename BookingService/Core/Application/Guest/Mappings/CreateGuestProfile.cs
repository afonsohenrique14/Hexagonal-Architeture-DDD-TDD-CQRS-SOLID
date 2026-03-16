
using Application.Guest.DTOs;
using AutoMapper;

namespace Application.Guest.Mappings;

public class CreateGuestProfile: Profile
{
    public CreateGuestProfile()
    {
        CreateMap<Domain.Guest.Entities.Guest, CreateGuestDTO>()
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
