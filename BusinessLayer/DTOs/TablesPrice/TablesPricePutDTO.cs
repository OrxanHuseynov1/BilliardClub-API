using Domain.Enums;

namespace BusinessLayer.DTOs.TablesPrice;

public class TablesPricePutDTO
{
    public TableType TableType { get; set; }
    public decimal HourlyPrice { get; set; }
}
