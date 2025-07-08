using BusinessLayer.DTOs.Table;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using BusinessLayer.Services.Abstractions;
using DAL.SqlServer.Context; 

namespace BusinessLayer.Services.Implementations;

public class TableService : ITableService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TableService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown User";
    }

    private Guid GetCurrentCompanyId()
    {
        var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        if (Guid.TryParse(companyIdClaim, out var companyId))
        {
            return companyId;
        }
        return Guid.Empty;
    }

    public async Task<List<TableGetDTO>> GetAllTablesAsync()
    {
        var companyId = GetCurrentCompanyId();
        var tables = await _context.Tables
                                   .Where(t => t.CompanyId == companyId && t.DeletedAt == null)
                                   .ToListAsync();

        var tableGetDTOs = _mapper.Map<List<TableGetDTO>>(tables);

        foreach (var dto in tableGetDTOs)
        {
            var tablesPrice = await _context.TablesPrices
                                            .Where(tp => tp.CompanyId == companyId && tp.TableType == dto.Type && tp.DeletedAt == null)
                                            .FirstOrDefaultAsync();
            dto.CurrentHourlyPrice = tablesPrice?.HourlyPrice; 
        }

        return tableGetDTOs;
    }

    public async Task<TableGetDTO> GetTableByIdAsync(Guid id)
    {
        var companyId = GetCurrentCompanyId();
        var table = await _context.Tables
                                  .Where(t => t.CompanyId == companyId && t.Id == id && t.DeletedAt == null)
                                  .FirstOrDefaultAsync();

        if (table == null) return null;

        var tableGetDTO = _mapper.Map<TableGetDTO>(table);

        var tablesPrice = await _context.TablesPrices
                                        .Where(tp => tp.CompanyId == companyId && tp.TableType == table.Type && tp.DeletedAt == null)
                                        .FirstOrDefaultAsync();
        tableGetDTO.CurrentHourlyPrice = tablesPrice?.HourlyPrice;

        return tableGetDTO;
    }

    public async Task<TableGetDTO> CreateTableAsync(TablePostDTO createDto)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var table = _mapper.Map<Table>(createDto);
        table.Id = Guid.NewGuid();
        table.CompanyId = companyId; 
        table.CreatedAt = DateTime.Now;
        table.CreatedBy = userId; 

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        var createdTableDto = _mapper.Map<TableGetDTO>(table);
        var tablesPrice = await _context.TablesPrices
                                        .Where(tp => tp.CompanyId == companyId && tp.TableType == table.Type && tp.DeletedAt == null)
                                        .FirstOrDefaultAsync();
        createdTableDto.CurrentHourlyPrice = tablesPrice?.HourlyPrice;

        return createdTableDto;
    }

    public async Task<TableGetDTO> UpdateTableAsync(Guid id, TablePutDTO updateDto)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var tableToUpdate = await _context.Tables
                                          .Where(t => t.CompanyId == companyId && t.Id == id && t.DeletedAt == null)
                                          .FirstOrDefaultAsync();
        if (tableToUpdate == null) return null;

        _mapper.Map(updateDto, tableToUpdate); 
        tableToUpdate.LastModifiedAt = DateTime.Now;
        tableToUpdate.LastModifiedBy = userId;

        await _context.SaveChangesAsync();

        var updatedTableDto = _mapper.Map<TableGetDTO>(tableToUpdate);
        var tablesPrice = await _context.TablesPrices
                                        .Where(tp => tp.CompanyId == companyId && tp.TableType == tableToUpdate.Type && tp.DeletedAt == null)
                                        .FirstOrDefaultAsync();
        updatedTableDto.CurrentHourlyPrice = tablesPrice?.HourlyPrice;

        return updatedTableDto;
    }

    public async Task<bool> DeleteTableAsync(Guid id)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var tableToDelete = await _context.Tables
                                          .Where(t => t.CompanyId == companyId && t.Id == id && t.DeletedAt == null)
                                          .FirstOrDefaultAsync();
        if (tableToDelete == null) return false;

        tableToDelete.DeletedAt = DateTime.Now;
        tableToDelete.DeletedBy = userId;

        await _context.SaveChangesAsync();
        return true;
    }
}