using AutoMapper;
using BusinessLayer.DTOs.Product;
using Domain.Entities;

namespace BusinessLayer.Profiles.ProductProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductGetDTO, Product>().ReverseMap();
        CreateMap<ProductPostDTO, Product>().ReverseMap();
        CreateMap<ProductPutDTO, Product>().ReverseMap();
    }
}
