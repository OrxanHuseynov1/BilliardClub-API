
using BusinessLayer.DTOs.Expenses;

namespace BusinessLayer.Services.Abstractions;

public interface IExpensesService
{
    Task<ExpensesGetDTO?> AddExpensesAsync(ExpensesPostDTO createDto);
    Task<bool> DeleteExpensesAsync(Guid id);
    Task<List<ExpensesGetDTO>> GetAllExpensesAsync();
    Task<ExpensesGetDTO?> GetExpensesByIdAsync(Guid id);
}