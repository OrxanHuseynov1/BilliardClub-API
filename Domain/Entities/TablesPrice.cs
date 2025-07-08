using Domain.Entities.Common;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class TablesPrice : AuditableEntity
{
    public TableType TableType { get; set; }
    public decimal HourlyPrice { get; set; } = 0;
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;
}
