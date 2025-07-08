using BusinessLayer.DTOs.Expenses; 
using BusinessLayer.Services.Abstractions; 
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilliardClub_App.Controllers;
[ApiController]
[Route("api/[controller]")] 
public class ExpensesController : ControllerBase
{
    private readonly IExpensesService _expensesService;

    public ExpensesController(IExpensesService expensesService)
    {
        _expensesService = expensesService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ExpensesGetDTO>>> GetAllExpenses()
    {
        var expenses = await _expensesService.GetAllExpensesAsync();
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExpensesGetDTO>> GetExpenseById(Guid id)
    {
        var expense = await _expensesService.GetExpensesByIdAsync(id);
        if (expense == null)
        {
            return NotFound("Xərc tapılmadı və ya silinib.");
        }
        return Ok(expense);
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExpensesGetDTO>> AddExpense([FromBody] ExpensesPostDTO createDto)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(ModelState);
        }

        var newExpense = await _expensesService.AddExpensesAsync(createDto);
        if (newExpense == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Xərc əlavə edilərkən xəta baş verdi.");
        }

        return CreatedAtAction(nameof(GetExpenseById), new { id = newExpense.Id }, newExpense);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteExpense(Guid id)
    {
        var result = await _expensesService.DeleteExpensesAsync(id);
        if (!result)
        {
            return NotFound("Xərc tapılmadı və ya artıq silinib.");
        }
        return NoContent(); // Uğurlu silmə üçün 204 No Content
    }
}