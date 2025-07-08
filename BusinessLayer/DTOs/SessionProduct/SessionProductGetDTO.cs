using System;
using BusinessLayer.DTOs.Product; 

namespace BusinessLayer.DTOs.SessionProduct;

public class SessionProductGetDTO
{
    public Guid Id { get; set; }
    public Guid TableSessionId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public ProductGetDTO? Product { get; set; } 
}