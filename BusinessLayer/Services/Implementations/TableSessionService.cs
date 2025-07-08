// BusinessLayer/Services/Implementations/TableSessionService.cs
using AutoMapper;
using BusinessLayer.DTOs.TableSession;
using DAL.SqlServer.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using BusinessLayer.Services.Abstractions;
using BusinessLayer.DTOs.SessionProduct;

namespace BusinessLayer.Services.Implementations;

public class TableSessionService : ITableSessionService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TableSessionService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId() =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown User";

    private Guid GetCurrentCompanyId()
    {
        var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        return Guid.TryParse(companyIdClaim, out var companyId) ? companyId : Guid.Empty;
    }

    public async Task<TableSessionGetDTO?> StartSessionAsync(TableSessionPostDTO dto)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == dto.TableId && t.CompanyId == companyId && t.DeletedAt == null);
        if (table == null) return null;

        if (table.IsActive)
        {
            return null; 
        }

        var session = new TableSession
        {
            Id = Guid.NewGuid(),
            TableId = dto.TableId,
            StartTime = DateTime.Now,
            HourlyPrice = dto.HourlyPrice,
            CompanyId = companyId,
            CreatedAt = DateTime.Now,
            CreatedBy = userId
        };

        _context.TableSessions.Add(session);

        table.IsActive = true;
        table.LastModifiedAt = DateTime.Now;
        table.LastModifiedBy = userId;

        await _context.SaveChangesAsync();

        return new TableSessionGetDTO
        {
            Id = session.Id,
            TableId = table.Id,
            TableName = table.TableName,
            StartTime = session.StartTime,
            EndTime = null,
            HourlyPrice = session.HourlyPrice,
            PaymentType = session.PaymentType, 
            SessionProducts = new List<SessionProductGetDTO>()
        };
    }

    public async Task<TableSessionGetDTO?> EndSessionAsync(Guid sessionId, TableSessionPutDTO dto)
    {
        var session = await _context.TableSessions
            .Include(s => s.Table)
            .Include(s => s.SessionProducts)
                .ThenInclude(sp => sp.Product)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.DeletedAt == null);

        if (session == null) return null;

        if (session.EndTime != null)
        {
            return null; 
        }

        session.EndTime = DateTime.Now;
        session.LastModifiedAt = DateTime.Now;
        session.LastModifiedBy = GetCurrentUserId();
        session.PaymentType = dto.PaymentType; 

        var table = session.Table;
        if (table != null)
        {
            table.IsActive = false;
            table.LastModifiedAt = DateTime.Now;
            table.LastModifiedBy = GetCurrentUserId();
        }

        await _context.SaveChangesAsync();

        var sessionDto = _mapper.Map<TableSessionGetDTO>(session);
        sessionDto.SessionProducts = _mapper.Map<List<SessionProductGetDTO>>(session.SessionProducts);
        return sessionDto;
    }

    public async Task<TableSessionGetDTO?> GetCurrentSessionForTableAsync(Guid tableId)
    {
        var companyId = GetCurrentCompanyId();
        if (companyId == Guid.Empty) return null;

        var session = await _context.TableSessions
            .Include(s => s.Table)
            .Include(s => s.SessionProducts.Where(sp => sp.DeletedAt == null))
                .ThenInclude(sp => sp.Product)
            .Where(s => s.TableId == tableId && s.CompanyId == companyId && s.EndTime == null && s.DeletedAt == null)
            .OrderByDescending(s => s.StartTime)
            .FirstOrDefaultAsync();

        if (session == null) return null;

        var sessionDto = _mapper.Map<TableSessionGetDTO>(session);
        sessionDto.SessionProducts = _mapper.Map<List<SessionProductGetDTO>>(session.SessionProducts);
        return sessionDto;
    }

    public async Task<IEnumerable<TableSessionGetDTO>> GetPastSessionsAsync(DateTime? filterByDate = null, string? tableName = null, int pageNumber = 1, int pageSize = 10)
    {
        var companyId = GetCurrentCompanyId();
        if (companyId == Guid.Empty) return new List<TableSessionGetDTO>();

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _context.TableSessions
            .Include(s => s.Table)
            .Include(s => s.SessionProducts.Where(sp => sp.DeletedAt == null))
                .ThenInclude(sp => sp.Product)
            .Where(s => s.CompanyId == companyId && s.EndTime != null && s.DeletedAt == null);

        if (filterByDate.HasValue)
        {
            query = query.Where(s => s.EndTime.HasValue &&
                                     s.EndTime.Value.Date == filterByDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(tableName))
        {
            query = query.Where(s => s.Table != null && s.Table.TableName.ToLower().Contains(tableName.ToLower()));
        }

        var sessions = await query
            .OrderByDescending(s => s.EndTime)
            .Skip((pageNumber - 1) * pageSize) 
            .Take(pageSize)                   
            .ToListAsync();

        return _mapper.Map<IEnumerable<TableSessionGetDTO>>(sessions);
    }

    public async Task<TableSessionGetDTO?> GetSessionDetailsAsync(Guid sessionId)
    {
        var companyId = GetCurrentCompanyId();
        if (companyId == Guid.Empty) return null;

        var session = await _context.TableSessions
            .Include(s => s.Table)
            .Include(s => s.SessionProducts.Where(sp => sp.DeletedAt == null))
                .ThenInclude(sp => sp.Product)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.CompanyId == companyId && s.DeletedAt == null);

        if (session == null) return null;

        var sessionDto = _mapper.Map<TableSessionGetDTO>(session);
        sessionDto.SessionProducts = _mapper.Map<List<SessionProductGetDTO>>(session.SessionProducts);
        return sessionDto;
    }
}