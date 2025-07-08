using AutoMapper;
using BusinessLayer.DTOs.User;
using BusinessLayer.Services.Abstractions;
using DAL.SqlServer.Context;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessLayer.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
    }

    private Guid GetCurrentCompanyId()
    {
        var companyId = _httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        return Guid.TryParse(companyId, out var id) ? id : Guid.Empty;
    }

    private RoleType GetCurrentUserRole()
    {
        var roleValue = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        return Enum.TryParse(roleValue, out RoleType role) ? role : RoleType.Seller;
    }

    public async Task<List<UserGetDTO>> GetAllUsersAsync()
    {
        var companyId = GetCurrentCompanyId();
        var users = await _context.Users
            .Where(u => u.CompanyId == companyId && u.DeletedAt == null)
            .ToListAsync();

        return _mapper.Map<List<UserGetDTO>>(users);
    }

    public async Task<UserGetDTO?> GetUserByIdAsync(Guid id)
    {
        var companyId = GetCurrentCompanyId();
        var user = await _context.Users
            .Where(u => u.Id == id && u.CompanyId == companyId && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        return user is null ? null : _mapper.Map<UserGetDTO>(user);
    }

    public async Task<UserGetDTO?> CreateUserAsync(UserPostDTO createDto)
    {
        var currentRole = GetCurrentUserRole();
        var companyId = GetCurrentCompanyId();
        var userId = GetCurrentUserId();

        if (currentRole != RoleType.Admin || createDto.Role != RoleType.Seller)
            return null; 

        var user = _mapper.Map<User>(createDto);
        user.Id = Guid.NewGuid();
        user.CompanyId = companyId;
        user.CreatedAt = DateTime.Now;
        user.CreatedBy = userId;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserGetDTO>(user);
    }

    public async Task<UserGetDTO?> UpdateUserAsync(Guid id, UserPutDTO updateDto)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var user = await _context.Users
            .Where(u => u.Id == id && u.CompanyId == companyId && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (user is null) return null;

        _mapper.Map(updateDto, user);
        user.LastModifiedAt = DateTime.Now;
        user.LastModifiedBy = userId;

        await _context.SaveChangesAsync();
        return _mapper.Map<UserGetDTO>(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var user = await _context.Users
            .Where(u => u.Id == id && u.CompanyId == companyId && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (user is null) return false;

        user.DeletedAt = DateTime.Now;
        user.DeletedBy = userId;

        await _context.SaveChangesAsync();
        return true;
    }
}
