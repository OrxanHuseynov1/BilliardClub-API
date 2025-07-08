using Domain.Enums;

namespace BusinessLayer.DTOs.TablesPrice;

public class TablesPriceGetDTO
{
    public Guid Id { get; set; }
    public TableType TableType { get; set; }
    public decimal HourlyPrice { get; set; }
}
