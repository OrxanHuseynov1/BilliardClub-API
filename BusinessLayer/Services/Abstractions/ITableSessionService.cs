using BusinessLayer.DTOs.TableSession;

namespace BusinessLayer.Services.Abstractions;

public interface ITableSessionService
{
    Task<TableSessionGetDTO?> StartSessionAsync(TableSessionPostDTO dto);
    Task<TableSessionGetDTO?> EndSessionAsync(Guid sessionId, TableSessionPutDTO dto);
    Task<TableSessionGetDTO?> GetCurrentSessionForTableAsync(Guid tableId);
    Task<IEnumerable<TableSessionGetDTO>> GetPastSessionsAsync(DateTime? filterByDate = null, string? tableName = null, int pageNumber = 1, int pageSize = 10);
    Task<TableSessionGetDTO?> GetSessionDetailsAsync(Guid sessionId);
}   