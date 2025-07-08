namespace BusinessLayer.DTOs.SessionProduct;

public class SessionProductPostDTO
{
    public Guid TableSessionId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
