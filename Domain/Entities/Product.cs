using Domain.Entities.Common;

namespace Domain.Entities;

public class Product : AuditableEntity
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int Count { get; set; }

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;

}
