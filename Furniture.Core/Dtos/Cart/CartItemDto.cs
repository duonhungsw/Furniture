namespace Furniture.Core;

public class CartItemDto
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public string? UrlImage { get; set; }
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool Status { get; set; }
    public decimal TotalMoney { get; set; }
    public int QuantityInStock { get; set; }
    public DateTime? CreatedAt { get; set; }
}