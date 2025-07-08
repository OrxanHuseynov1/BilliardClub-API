using BusinessLayer.DTOs.Table;
using BusinessLayer.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Bu using directive-ə ehtiyac yoxdur, GetCurrentUserId/CompanyId metodları service-ə köçürülüb.

namespace BilliardClub_App.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService = tableService;
    }

    // GetCurrentUserId() və GetCurrentCompanyId() metodları artıq TableService içindədir.
    // Controllerdə bunlara ehtiyac yoxdur, çünki servis özü istifadəçi məlumatlarını HttpContextAccessor vasitəsilə əldə edir.
    // Bu metodları Controllerdən silə bilərik, əgər başqa yerdə istifadə olunmurlarsa.
    /*
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
    */

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TableGetDTO>>> GetTables()
    {
        // Service artıq özü companyId-ni və lazımi məlumatları (ActiveSession daxil olmaqla) çəkəcək.
        var tables = await _tableService.GetAllTablesAsync();
        return Ok(tables);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TableGetDTO>> GetTable(Guid id)
    {
        var table = await _tableService.GetTableByIdAsync(id);
        if (table == null)
        {
            return NotFound();
        }
        return Ok(table);
    }

    [HttpPost]
    public async Task<ActionResult<TableGetDTO>> PostTable(TablePostDTO createDto)
    {
        var table = await _tableService.CreateTableAsync(createDto);
        // CreatedAtAction-a göndərilən 'table' obyekti özündə Id-ni saxlamalıdır.
        // Həmçinin, HttpContext.Response.StatusCode = 201 olacaq.
        return CreatedAtAction(nameof(GetTable), new { id = table.Id }, table);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTable(Guid id, TablePutDTO updateDto)
    {
        var updatedTable = await _tableService.UpdateTableAsync(id, updateDto);
        if (updatedTable == null)
        {
            return NotFound();
        }
        // OK (200) statusu ilə yenilənmiş obyekti qaytarırıq.
        return Ok(updatedTable);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTable(Guid id)
    {
        var result = await _tableService.DeleteTableAsync(id);
        if (!result)
        {
            return NotFound(); // Əgər tapılmasa və ya silinməsə
        }
        // 204 No Content statusu ilə uğurlu silinmə mesajı.
        return NoContent();
    }
}