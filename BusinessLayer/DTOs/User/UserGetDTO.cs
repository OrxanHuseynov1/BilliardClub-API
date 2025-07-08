using Domain.Enums;

namespace BusinessLayer.DTOs.User;

public class UserGetDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Code { get; set; }
    public RoleType Role { get; set; } 
}
