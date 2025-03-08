namespace Furniture.Core;

public class OrderCheckout
{
	public Guid ProductId { get; set; }
	public string? ProductName { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	public decimal TotalMoney { get; set; }
}
