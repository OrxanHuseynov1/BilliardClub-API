using AutoMapper;
using BusinessLayer.DTOs.Table;
using Domain.Entities;

namespace BusinessLayer.Profiles.TableProfiles;

public class TableProfile : Profile
{
    public TableProfile()
    {
        CreateMap<TableGetDTO, Table>().ReverseMap();
        CreateMap<TablePostDTO, Table>().ReverseMap();
        CreateMap<TablePutDTO, Table>().ReverseMap();
    }
}
