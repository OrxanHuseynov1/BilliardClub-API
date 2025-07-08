namespace BusinessLayer.DTOs.Expenses;

public class ExpensesGetDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; } 
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } 
    public Guid CompanyId { get; set; }
}
