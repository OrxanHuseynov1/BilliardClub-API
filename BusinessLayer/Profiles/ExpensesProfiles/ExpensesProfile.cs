using AutoMapper;
using BusinessLayer.DTOs.Expenses;
using Domain.Entities;

namespace BusinessLayer.Profiles.ExpensesProfiles;

public class ExpensesProfile : Profile
{
    public ExpensesProfile()
    {
        CreateMap<Expenses, ExpensesGetDTO>();
        CreateMap<ExpensesPostDTO, Expenses>();
    }
}