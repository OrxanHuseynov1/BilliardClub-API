using AutoMapper;
using BusinessLayer.DTOs.Company;
using Domain.Entities;

namespace BusinessLayer.Profiles.CompanyProfiles;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<Company, CompanyGetDTO>().ReverseMap();
    }
}
