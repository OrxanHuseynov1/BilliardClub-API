using Domain.Entities.Common;

namespace Domain.Entities;

public class Expenses : AuditableEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;

}
