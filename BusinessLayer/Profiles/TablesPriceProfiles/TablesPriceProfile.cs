using AutoMapper;
using BusinessLayer.DTOs.TablesPrice;
using Domain.Entities;

namespace BusinessLayer.Profiles.TablesPriceProfiles;

public class TablesPriceProfile : Profile
{
    public TablesPriceProfile() 
    {
        CreateMap<TablesPriceGetDTO, TablesPrice>().ReverseMap();
        CreateMap<TablesPricePutDTO, TablesPrice>().ReverseMap();
        CreateMap<TablesPricePostDTO, TablesPrice>().ReverseMap();
    } 
}
