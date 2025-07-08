using AutoMapper;
using BusinessLayer.DTOs.Auth;
using BusinessLayer.ExternalServices.Abstractions;
using BusinessLayer.Services.Abstractions;
using DAL.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(AppDbContext context, IMapper mapper, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _mapper = mapper;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Code == request.Code && !x.IsDeleted) ?? throw new Exception("İstifadəçi tapılmadı.");
        var token = await _jwtTokenService.GenerateJwtToken(user);

        return new LoginResponseDTO
        {
            Token = token
        };
    }
}
