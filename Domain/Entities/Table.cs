using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Table : AuditableEntity
{
    public string TableName { get; set; } = default!;
    public TableType Type { get; set; }
    public bool IsActive { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;
}
