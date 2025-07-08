using AutoMapper;
using BusinessLayer.DTOs.Expenses;
using BusinessLayer.Services.Abstractions;
using DAL.SqlServer.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessLayer.Services.Implementations;

public class ExpensesService : IExpensesService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExpensesService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        return !string.IsNullOrEmpty(userId) ? userId : "SystemUser";
    }

    private Guid GetCurrentCompanyId()
    {
        var companyId = _httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        return Guid.TryParse(companyId, out var id) ? id : Guid.Empty;
    }

    public async Task<ExpensesGetDTO?> AddExpensesAsync(ExpensesPostDTO createDto)
    {
        var currentCompanyId = GetCurrentCompanyId();
        var expense = _mapper.Map<Expenses>(createDto);
        var userId = GetCurrentUserId();
        expense.CompanyId = currentCompanyId;
        expense.CreatedAt = DateTime.Now;
        expense.CreatedBy = userId;

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return _mapper.Map<ExpensesGetDTO>(expense);
    }

    public async Task<bool> DeleteExpensesAsync(Guid id)
    {
        var userId = GetCurrentUserId();
        var currentCompanyId = GetCurrentCompanyId();
        var expenseToDelete = await _context.Expenses
            .Where(e => e.Id == id && e.CompanyId == currentCompanyId && !e.IsDeleted)
            .FirstOrDefaultAsync();


        expenseToDelete!.DeletedAt = DateTime.Now;
        expenseToDelete.DeletedBy = userId;

        if (expenseToDelete is null)
        {
            return false;
        }

        _context.Entry(expenseToDelete).State = EntityState.Deleted;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ExpensesGetDTO>> GetAllExpensesAsync()
    {
        var currentCompanyId = GetCurrentCompanyId();
        var expenses = await _context.Expenses
            .Where(e => e.CompanyId == currentCompanyId && !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<ExpensesGetDTO>>(expenses);
    }

    public async Task<ExpensesGetDTO?> GetExpensesByIdAsync(Guid id)
    {
        var currentCompanyId = GetCurrentCompanyId();
        var expense = await _context.Expenses
            .Where(e => e.Id == id && e.CompanyId == currentCompanyId && !e.IsDeleted)
            .FirstOrDefaultAsync();

        return expense is null ? null : _mapper.Map<ExpensesGetDTO>(expense);
    }
}