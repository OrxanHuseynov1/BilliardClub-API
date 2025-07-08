using BusinessLayer.DTOs.TablesPrice;
using BusinessLayer.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BilliardClub_App.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class TablesPricesController : ControllerBase
{
    private readonly ITablesPriceService _tablesPriceService;

    public TablesPricesController(ITablesPriceService tablesPriceService)
    {
        _tablesPriceService = tablesPriceService;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
    }

    private Guid GetCurrentCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        if (Guid.TryParse(companyIdClaim, out var companyId))
        {
            return companyId;
        }
        return Guid.Empty;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TablesPriceGetDTO>>> GetTablesPrices()
    {
        var prices = await _tablesPriceService.GetAllTablesPricesAsync();
        return Ok(prices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TablesPriceGetDTO>> GetTablesPrice(Guid id)
    {
        var price = await _tablesPriceService.GetTablesPriceByIdAsync(id);
        if (price == null)
        {
            return NotFound(); 
        }
        return Ok(price); 
    }

    [HttpPost]
    public async Task<ActionResult<TablesPriceGetDTO>> PostTablesPrice(TablesPricePostDTO createDto)
    {
        var tablesPrice = await _tablesPriceService.CreateTablesPriceAsync(createDto);
        return CreatedAtAction(nameof(GetTablesPrice), new { id = tablesPrice.Id }, tablesPrice); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTablesPrice(Guid id, TablesPricePutDTO updateDto)
    {
        var updatedPrice = await _tablesPriceService.UpdateTablesPriceAsync(id, updateDto);
        if (updatedPrice == null)
        {
            return NotFound(); 
        }
        return NoContent(); 
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTablesPrice(Guid id)
    {
        var result = await _tablesPriceService.DeleteTablesPriceAsync(id);
        if (!result)
        {
            return NotFound(); 
        }
        return NoContent();
    }


    [HttpPost("initialize-default")]
    public async Task<IActionResult> InitializeDefaultTablesPrices()
    {
        var companyId = GetCurrentCompanyId();
        var createdBy = GetCurrentUserId();

        if (companyId == Guid.Empty)
        {
            return BadRequest("Company ID could not be retrieved from token. Ensure user is logged in and token is valid.");
        }

        await _tablesPriceService.InitializeDefaultTablesPrices(companyId, createdBy);
        return Ok("Default table prices initialized or already exist for this company.");
    }
}