using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class User : AuditableEntity
{
    public string Name { get; set; } = default!;
    public int Code { get; set; }
    public RoleType Role { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;
}
