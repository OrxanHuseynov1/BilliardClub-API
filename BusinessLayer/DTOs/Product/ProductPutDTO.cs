namespace BusinessLayer.DTOs.Product;

public class ProductPutDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int Count { get; set; }
    public Guid CompanyId { get; set; }
}
