// BusinessLayer/Profiles/TableSessionProfiles/TableSessionProfile.cs
using AutoMapper;
using BusinessLayer.DTOs.TableSession;
using Domain.Entities;
using Domain.Enums; // PaymentType enum-u üçün namespace əlavə edildi

namespace BusinessLayer.Profiles.TableSessionProfiles;

public class TableSessionProfile : Profile
{
    public TableSessionProfile()
    {
        CreateMap<TableSession, TableSessionGetDTO>()
            .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Table.TableName))
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType)); 


        CreateMap<TableSessionPostDTO, TableSession>()
            .ForMember(dest => dest.HourlyPrice, opt => opt.MapFrom(src => src.HourlyPrice))
            .ReverseMap();

        CreateMap<TableSessionPutDTO, TableSession>();
    }
}