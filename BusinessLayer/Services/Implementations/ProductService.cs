using AutoMapper;
using BusinessLayer.DTOs.Product;
using BusinessLayer.Services.Abstractions;
using DAL.SqlServer.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessLayer.Services.Implementations;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
    }

    private Guid GetCurrentCompanyId()
    {
        var companyId = _httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        return Guid.TryParse(companyId, out var id) ? id : Guid.Empty;
    }

    public async Task<List<ProductGetDTO>> GetAllProductsAsync()
    {
        var companyId = GetCurrentCompanyId();
        var products = await _context.Product
            .Where(p => p.CompanyId == companyId && p.DeletedAt == null)
            .ToListAsync();

        return _mapper.Map<List<ProductGetDTO>>(products);
    }

    public async Task<ProductGetDTO?> GetProductByIdAsync(Guid id)
    {
        var companyId = GetCurrentCompanyId();
        var product = await _context.Product
            .Where(p => p.Id == id && p.CompanyId == companyId && p.DeletedAt == null)
            .FirstOrDefaultAsync();

        return product is null ? null : _mapper.Map<ProductGetDTO>(product);
    }

    public async Task<ProductGetDTO> CreateProductAsync(ProductPostDTO createDto)
    {
        var companyId = GetCurrentCompanyId();
        var userId = GetCurrentUserId();

        var product = _mapper.Map<Product>(createDto);
        product.Id = Guid.NewGuid();
        product.CompanyId = companyId;
        product.CreatedAt = DateTime.Now;
        product.CreatedBy = userId;

        _context.Product.Add(product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductGetDTO>(product);
    }

    public async Task<ProductGetDTO?> UpdateProductAsync(Guid id, ProductPutDTO updateDto)
    {
        var companyId = GetCurrentCompanyId();
        var userId = GetCurrentUserId();

        var product = await _context.Product
            .Where(p => p.Id == id && p.CompanyId == companyId && p.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (product is null) return null;

        _mapper.Map(updateDto, product);
        product.LastModifiedAt = DateTime.Now;
        product.LastModifiedBy = userId;

        await _context.SaveChangesAsync();

        return _mapper.Map<ProductGetDTO>(product);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var companyId = GetCurrentCompanyId();
        var userId = GetCurrentUserId();

        var product = await _context.Product
            .Where(p => p.Id == id && p.CompanyId == companyId && p.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (product is null) return false;

        product.DeletedAt = DateTime.Now;
        product.DeletedBy = userId;

        await _context.SaveChangesAsync();
        return true;
    }
}
