namespace BusinessLayer.DTOs.Product;

public class ProductPostDTO
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int Count { get; set; }
}
