using BusinessLayer.DTOs.Product;

namespace BusinessLayer.Services.Abstractions;

public interface IProductService
{
    Task<List<ProductGetDTO>> GetAllProductsAsync();
    Task<ProductGetDTO?> GetProductByIdAsync(Guid id);
    Task<ProductGetDTO> CreateProductAsync(ProductPostDTO createDto);
    Task<ProductGetDTO?> UpdateProductAsync(Guid id, ProductPutDTO updateDto);
    Task<bool> DeleteProductAsync(Guid id);
}
