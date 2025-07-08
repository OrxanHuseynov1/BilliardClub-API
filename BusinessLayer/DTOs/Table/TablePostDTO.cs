using Domain.Enums;

namespace BusinessLayer.DTOs.Table;

public class TablePostDTO
{
    public string TableName { get; set; } = default!;
    public TableType Type { get; set; }
    public bool IsActive { get; set; }
}
