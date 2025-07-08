using Domain.Enums;


namespace BusinessLayer.DTOs.Table;

public class TableGetDTO
{
    public Guid Id { get; set; }
    public string TableName { get; set; } = default!;
    public TableType Type { get; set; } 
    public bool IsActive { get; set; }
    public Guid CompanyId { get; set; }
    public decimal? CurrentHourlyPrice { get; set; }
}
