using BusinessLayer.DTOs.User;

namespace BusinessLayer.Services.Abstractions;

public interface IUserService
{
    Task<List<UserGetDTO>> GetAllUsersAsync();
    Task<UserGetDTO?> GetUserByIdAsync(Guid id);
    Task<UserGetDTO?> CreateUserAsync(UserPostDTO createDto);
    Task<UserGetDTO?> UpdateUserAsync(Guid id, UserPutDTO updateDto);
    Task<bool> DeleteUserAsync(Guid id);
}
