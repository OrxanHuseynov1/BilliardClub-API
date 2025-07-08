using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class TableSession : AuditableEntity
{
    public Guid TableId { get; set; }
    public Table Table { get; set; } = default!;

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; } 

    public decimal HourlyPrice { get; set; } 
    public PaymentType PaymentType { get; set; }
 
    public ICollection<SessionProduct> SessionProducts { get; set; } = [];
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;
}
