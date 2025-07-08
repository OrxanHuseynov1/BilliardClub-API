using BusinessLayer.DTOs.TablesPrice; // Sizin DTO-larınız
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DAL.SqlServer.Context;
using BusinessLayer.Services.Abstractions;


namespace BusinessLayer.Services.Implementations;

public class TablesPriceService : ITablesPriceService
{
    private readonly AppDbContext _context; 
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor; 

    public TablesPriceService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
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

    public async Task<List<TablesPriceGetDTO>> GetAllTablesPricesAsync()
    {
        var companyId = GetCurrentCompanyId();
        var prices = await _context.TablesPrices
                                   .Where(tp => tp.CompanyId == companyId && tp.DeletedAt == null) 
                                   .ToListAsync();
        return _mapper.Map<List<TablesPriceGetDTO>>(prices);
    }

    public async Task<TablesPriceGetDTO> GetTablesPriceByIdAsync(Guid id)
    {
        var companyId = GetCurrentCompanyId();
        var price = await _context.TablesPrices
                                  .Where(tp => tp.CompanyId == companyId && tp.Id == id && tp.DeletedAt == null)
                                  .FirstOrDefaultAsync();
        return _mapper.Map<TablesPriceGetDTO>(price);
    }

    public async Task<TablesPriceGetDTO> CreateTablesPriceAsync(TablesPricePostDTO createDto)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var tablesPrice = _mapper.Map<TablesPrice>(createDto);
        tablesPrice.Id = Guid.NewGuid(); 
        tablesPrice.CompanyId = companyId;
        tablesPrice.CreatedAt = DateTime.Now;
        tablesPrice.CreatedBy = userId;

        _context.TablesPrices.Add(tablesPrice);
        await _context.SaveChangesAsync();
        return _mapper.Map<TablesPriceGetDTO>(tablesPrice);
    }

    public async Task<TablesPriceGetDTO> UpdateTablesPriceAsync(Guid id, TablesPricePutDTO updateDto)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var tablesPriceToUpdate = await _context.TablesPrices
                                                .Where(tp => tp.CompanyId == companyId && tp.Id == id && tp.DeletedAt == null)
                                                .FirstOrDefaultAsync();
        if (tablesPriceToUpdate == null) return null;

        _mapper.Map(updateDto, tablesPriceToUpdate);
        tablesPriceToUpdate.LastModifiedAt = DateTime.Now;
        tablesPriceToUpdate.LastModifiedBy = userId;

        await _context.SaveChangesAsync();
        return _mapper.Map<TablesPriceGetDTO>(tablesPriceToUpdate);
    }

    public async Task<bool> DeleteTablesPriceAsync(Guid id)
    {
        var userId = GetCurrentUserId();
        var companyId = GetCurrentCompanyId();

        var tablesPriceToDelete = await _context.TablesPrices
                                                .Where(tp => tp.CompanyId == companyId && tp.Id == id && tp.DeletedAt == null)
                                                .FirstOrDefaultAsync();
        if (tablesPriceToDelete == null) return false;

        tablesPriceToDelete.DeletedAt = DateTime.Now;
        tablesPriceToDelete.DeletedBy = userId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task InitializeDefaultTablesPrices(Guid companyId, string createdBy)
    {
        var defaultPricesExist = await _context.TablesPrices
                                               .AnyAsync(tp => tp.CompanyId == companyId);

        if (!defaultPricesExist)
        {
            var defaultTypes = Enum.GetValues(typeof(TableType)).Cast<TableType>();
            var newPrices = new List<TablesPrice>();

            foreach (var type in defaultTypes)
            {
                newPrices.Add(new TablesPrice
                {
                    Id = Guid.NewGuid(),
                    TableType = type,
                    HourlyPrice = 0, 
                    CompanyId = companyId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                });
            }

            _context.TablesPrices.AddRange(newPrices);
            await _context.SaveChangesAsync();
        }
    }
}