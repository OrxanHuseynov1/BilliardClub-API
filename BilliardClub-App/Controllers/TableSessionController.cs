// BilliardClub_App/Controllers/TableSessionController.cs
using BusinessLayer.DTOs.SessionProduct;
using BusinessLayer.DTOs.TableSession; // TableSessionPutDTO üçün bunu istifadə edəcəyik
using BusinessLayer.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums; // PaymentType enum üçün əlavə etdik

namespace BilliardClub_App.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TableSessionController : ControllerBase
{
    private readonly ITableSessionService _tableSessionService;
    private readonly ISessionProductService _sessionProductService; // Doğru service istifadə olunduğundan əmin oluruq

    public TableSessionController(ITableSessionService tableSessionService, ISessionProductService sessionProductService)
    {
        _tableSessionService = tableSessionService;
        _sessionProductService = sessionProductService;
    }

    // --- TableSession Metodları ---

    [HttpPost("start")]
    [Authorize(Roles = "Admin,Seller")] // Rolları əlavə edirik
    [ProducesResponseType(200, Type = typeof(TableSessionGetDTO))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> StartSession([FromBody] TableSessionPostDTO dto)
    {
        var result = await _tableSessionService.StartSessionAsync(dto);
        if (result == null)
        {
            return BadRequest("Masa tapılmadı, aktiv sessiya mövcuddur və ya sorğu etibarsızdır.");
        }
        return Ok(result);
    }

    [HttpPut("end/{sessionId}")]
    [Authorize(Roles = "Admin,Seller")] // Rolları əlavə edirik
    [ProducesResponseType(200, Type = typeof(TableSessionGetDTO))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    // EndSession metodunu TableSessionPutDTO qəbul edəcək şəkildə dəyişirik
    public async Task<IActionResult> EndSession(Guid sessionId, [FromBody] TableSessionPutDTO dto)
    {
        if (!ModelState.IsValid) // Əgər DTO doğru gəlməyibsə
        {
            return BadRequest(ModelState);
        }

        // Service metodunu yenilənmiş imza ilə çağırırıq
        var endedSessionDetails = await _tableSessionService.EndSessionAsync(sessionId, dto);
        if (endedSessionDetails == null)
        {
            return NotFound("Sessiya tapılmadı və ya artıq bitib.");
        }
        return Ok(endedSessionDetails);
    }

    [HttpGet("table/{tableId}/current")]
    [Authorize(Roles = "Admin,Seller")] // Rolları əlavə edirik
    [ProducesResponseType(200, Type = typeof(TableSessionGetDTO))]
    [ProducesResponseType(204)] // No Content üçün
    public async Task<IActionResult> GetCurrentSessionForTable(Guid tableId)
    {
        if (tableId == Guid.Empty)
        {
            return BadRequest(new { message = "Masa ID-si boş ola bilməz." });
        }
        var session = await _tableSessionService.GetCurrentSessionForTableAsync(tableId);
        if (session == null)
        {
            // Aktiv sessiya yoxdursa 204 No Content qaytarmaq daha məqsədəuyğundur.
            return NoContent();
        }
        return Ok(session);
    }

    // --- SessionProduct Metodları ---

    [HttpPost("product/add-or-update")]
    [Authorize(Roles = "Admin,Seller")] // Rolları əlavə edirik
    [ProducesResponseType(200, Type = typeof(SessionProductGetDTO))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddOrUpdateProduct([FromBody] SessionProductPostDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Artıq AddOrUpdateProductAsync metodu ISessionProductService-dədir.
            var result = await _sessionProductService.AddOrUpdateProductAsync(dto);
            if (result == null)
            {
                return BadRequest("Məhsul əlavə/yenilə edilmədi. Səbəblər: Məhsul/sessiya tapılmadı, stok azdır, miqdar etibarsızdır, və ya məhsul silindi.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddOrUpdateProduct: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("product/delete/{sessionProductId}")]
    [Authorize(Roles = "Admin,Seller")] // Rolları əlavə edirik
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProduct(Guid sessionProductId)
    {
        try
        {
            // Artıq DeleteProductAsync metodu ISessionProductService-dədir.
            var success = await _sessionProductService.DeleteProductAsync(sessionProductId);
            if (!success)
            {
                return NotFound("Məhsul tapılmadı, artıq silinib və ya silinə bilmədi.");
            }
            return NoContent(); // 204 No Content - Silmə əməliyyatı uğurla tamamlandı
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteProduct: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("products/{tableSessionId}")]
    [Authorize(Roles = "Admin,Seller")] // Rolları əlavə edirik
    [ProducesResponseType(200, Type = typeof(IEnumerable<SessionProductGetDTO>))]
    public async Task<IActionResult> GetSessionProducts(Guid tableSessionId)
    {
        try
        {
            // Artıq GetSessionProductsAsync metodu ISessionProductService-dədir.
            var products = await _sessionProductService.GetSessionProductsAsync(tableSessionId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetSessionProducts: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("past-sessions")]
    [Authorize(Roles = "Admin,Seller")] // Rolları əlavə edirik
    [ProducesResponseType(200, Type = typeof(IEnumerable<TableSessionGetDTO>))]
    public async Task<IActionResult> GetPastSessions(
        [FromQuery] DateTime? filterByDate,
        [FromQuery] string? tableName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var sessions = await _tableSessionService.GetPastSessionsAsync(filterByDate, tableName, pageNumber, pageSize);
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPastSessions: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{sessionId}")]
    [Authorize(Roles = "Admin,Seller")] // Rolları əlavə edirik
    [ProducesResponseType(200, Type = typeof(TableSessionGetDTO))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSessionDetails(Guid sessionId)
    {
        try
        {
            var session = await _tableSessionService.GetSessionDetailsAsync(sessionId);
            if (session == null)
            {
                return NotFound("Sessiya tapılmadı.");
            }
            return Ok(session);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetSessionDetails: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}