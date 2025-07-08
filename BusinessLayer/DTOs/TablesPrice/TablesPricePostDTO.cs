using Domain.Enums;

namespace BusinessLayer.DTOs.TablesPrice;

public class TablesPricePostDTO
{
    public TableType TableType { get; set; }
    public decimal HourlyPrice { get; set; }
}
