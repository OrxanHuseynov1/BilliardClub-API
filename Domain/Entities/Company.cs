using Domain.Entities.Common;

namespace Domain.Entities;

public class Company : AuditableEntity
{
    public string Name { get; set; } = default!;

}
