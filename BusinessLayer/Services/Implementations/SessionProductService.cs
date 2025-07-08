// SessionProductService.cs
using AutoMapper;
using BusinessLayer.DTOs.SessionProduct;
using BusinessLayer.Services.Abstractions;
using DAL.SqlServer.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessLayer.Services.Implementations;

public class SessionProductService : ISessionProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionProductService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    // --- Köməkçi Metodlar ---
    private Guid GetCurrentCompanyId()
    {
        var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        return Guid.TryParse(companyIdClaim, out var id) ? id : Guid.Empty;
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
    }

    // --- Əsas Metodlar ---

    public async Task<SessionProductGetDTO?> AddOrUpdateProductAsync(SessionProductPostDTO dto)
    {
        var companyId = GetCurrentCompanyId();
        var userId = GetCurrentUserId();

        if (companyId == Guid.Empty)
        {
            return null;
        }

        // Product'ı tapın
        var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.CompanyId == companyId && p.DeletedAt == null);
        if (product == null)
        {
            return null;
        }

        // Sessiyanı tapın və aktiv olduğundan əmin olun
        var session = await _context.TableSessions.FirstOrDefaultAsync(s => s.Id == dto.TableSessionId && s.CompanyId == companyId && s.DeletedAt == null && s.EndTime == null);
        if (session == null)
        {
            return null; // Sessiya tapılmadı, silinib və ya bitib
        }

        var existingSessionProduct = await _context.SessionProducts
            .FirstOrDefaultAsync(sp => sp.TableSessionId == dto.TableSessionId && sp.ProductId == dto.ProductId && sp.CompanyId == companyId);

        // Nə qədər stok dəyişikliyinə ehtiyac var
        int quantityChange = 0;
        SessionProduct currentSessionProduct;

        // Miqdar 0 və ya mənfi olarsa soft delete et
        if (dto.Quantity <= 0)
        {
            if (existingSessionProduct != null && existingSessionProduct.DeletedAt == null) // Yalnız aktiv olanı silə bilərik
            {
                // Stok miqdarını geri qaytar
                quantityChange = existingSessionProduct.Quantity;
                product.Count += quantityChange;
                product.LastModifiedAt = DateTime.UtcNow;
                product.LastModifiedBy = userId;

                existingSessionProduct.DeletedAt = DateTime.UtcNow; // Soft delete
                existingSessionProduct.DeletedBy = userId;
                existingSessionProduct.LastModifiedAt = DateTime.UtcNow;
                existingSessionProduct.LastModifiedBy = userId;

                await _context.SaveChangesAsync();
            }
            return null; // Məhsul silindiyi üçün null qaytarırıq
        }

        if (existingSessionProduct == null || existingSessionProduct.DeletedAt != null) // Yeni məhsul əlavə et və ya soft deleted olanı aktivləşdir
        {
            // Yeni məhsul əlavə et (əgər miqdar > 0 olarsa)
            currentSessionProduct = existingSessionProduct ?? _mapper.Map<SessionProduct>(dto);

            if (existingSessionProduct == null) // Tamamilə yeni obyektdirsə
            {
                currentSessionProduct.Id = Guid.NewGuid();
                currentSessionProduct.CompanyId = companyId;
                currentSessionProduct.CreatedAt = DateTime.UtcNow;
                currentSessionProduct.CreatedBy = userId;
                _context.SessionProducts.Add(currentSessionProduct);
                quantityChange = dto.Quantity; // Yeni əlavə olunduğu üçün bütün miqdar stokdan çıxarılacaq
            }
            else // Soft deleted olan məhsul yenidən aktivləşdirilirsə
            {
                quantityChange = dto.Quantity - currentSessionProduct.Quantity; // Yeni miqdar ilə köhnə arasındakı fərq
                currentSessionProduct.Quantity = dto.Quantity; // Miqdarı yenilə
                currentSessionProduct.LastModifiedAt = DateTime.UtcNow;
                currentSessionProduct.LastModifiedBy = userId;
                currentSessionProduct.DeletedAt = null; // Aktivləşdir
                currentSessionProduct.DeletedBy = null;
            }

            // Stok yoxlaması
            if (product.Count < quantityChange)
            {
                return null; // Stokda kifayət qədər məhsul yoxdur
            }

            // Stokdan çıxarırıq
            product.Count -= quantityChange;
            product.LastModifiedAt = DateTime.UtcNow;
            product.LastModifiedBy = userId;

            await _context.SaveChangesAsync();
            return _mapper.Map<SessionProductGetDTO>(currentSessionProduct);
        }
        else // Mövcud məhsulun miqdarını yenilə (existingSessionProduct != null && existingSessionProduct.DeletedAt == null)
        {
            quantityChange = dto.Quantity - existingSessionProduct.Quantity;

            // Stok yoxlaması
            if (quantityChange > 0 && product.Count < quantityChange)
            {
                return null; // Stokda kifayət qədər məhsul yoxdur
            }

            existingSessionProduct.Quantity = dto.Quantity;
            existingSessionProduct.UnitPrice = dto.UnitPrice; // DTO-dan UnitPrice gəldiyini fərz edirəm
            existingSessionProduct.LastModifiedAt = DateTime.UtcNow;
            existingSessionProduct.LastModifiedBy = userId;
            existingSessionProduct.DeletedAt = null; // Əgər əvvəl soft delete edilmişdisə və indi miqdar artırılırsa, aktiv et

            // Stok miqdarını tənzimlə (artır və ya azaldır)
            product.Count -= quantityChange;
            product.LastModifiedAt = DateTime.UtcNow;
            product.LastModifiedBy = userId;

            await _context.SaveChangesAsync();
            return _mapper.Map<SessionProductGetDTO>(existingSessionProduct);
        }
    }

    public async Task<bool> DeleteProductAsync(Guid sessionProductId)
    {
        var companyId = GetCurrentCompanyId();
        if (companyId == Guid.Empty) return false;

        var userId = GetCurrentUserId();

        var sessionProduct = await _context.SessionProducts
            .Include(sp => sp.Product) // Product-a daxil etməyi unutmayın!
            .FirstOrDefaultAsync(sp => sp.Id == sessionProductId && sp.CompanyId == companyId && sp.DeletedAt == null); // Yalnız aktiv olanı silə bilərik

        if (sessionProduct == null) return false;

        // Stok miqdarını geri qaytar
        sessionProduct.Product.Count += sessionProduct.Quantity;
        sessionProduct.Product.LastModifiedAt = DateTime.UtcNow;
        sessionProduct.Product.LastModifiedBy = userId;

        // SessionProduct-ı soft delete et
        sessionProduct.DeletedAt = DateTime.UtcNow;
        sessionProduct.DeletedBy = userId;
        sessionProduct.LastModifiedAt = DateTime.UtcNow; // Son dəyişiklik tarixi
        sessionProduct.LastModifiedBy = userId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<SessionProductGetDTO>> GetSessionProductsAsync(Guid tableSessionId)
    {
        var companyId = GetCurrentCompanyId();
        if (companyId == Guid.Empty) return new List<SessionProductGetDTO>();

        var sessionProducts = await _context.SessionProducts
            .Where(sp => sp.TableSessionId == tableSessionId && sp.CompanyId == companyId && sp.DeletedAt == null) // Soft delete filtresi vacibdir
            .Include(sp => sp.Product)
            .ToListAsync();

        return _mapper.Map<List<SessionProductGetDTO>>(sessionProducts);
    }
}