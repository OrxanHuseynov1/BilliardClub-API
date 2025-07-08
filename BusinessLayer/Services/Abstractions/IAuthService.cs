using BusinessLayer.DTOs.Auth;

namespace BusinessLayer.Services.Abstractions;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
}
