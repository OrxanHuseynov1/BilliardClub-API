using Domain.Entities;

namespace BusinessLayer.ExternalServices.Abstractions;

public interface IJwtTokenService
{
    Task<string> GenerateJwtToken(User user);
}
