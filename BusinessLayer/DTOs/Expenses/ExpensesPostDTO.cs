
namespace BusinessLayer.DTOs.Expenses;

public class ExpensesPostDTO
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
}