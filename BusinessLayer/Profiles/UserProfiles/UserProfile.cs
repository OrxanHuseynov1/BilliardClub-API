using AutoMapper;
using BusinessLayer.DTOs.User;
using Domain.Entities;

namespace BusinessLayer.Profiles.UserProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserGetDTO, User>().ReverseMap();
        CreateMap<UserPostDTO, User>().ReverseMap();
        CreateMap<UserPutDTO, User>().ReverseMap();
    }
}
