using AutoMapper;
using BusinessLayer.DTOs.Product;
using BusinessLayer.DTOs.SessionProduct;
using Domain.Entities;

namespace BusinessLayer.Profiles.SessionProductProfiles;

public class SessionProductProfile : Profile
{
    public SessionProductProfile()
    {
        CreateMap<SessionProduct, SessionProductGetDTO>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

        CreateMap<Product, ProductGetDTO>();

        CreateMap<SessionProductPostDTO, SessionProduct>().ReverseMap();
    }
}