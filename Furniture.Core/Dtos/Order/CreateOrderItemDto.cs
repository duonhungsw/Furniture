using Furniture.Core.Dtos.Product;

namespace Furniture.Core.Dtos.Order;

public class CreateOrderItemDto
{
	public Guid OrderId { get; set; }
	public Guid ProductId { get; set; }
	public int Quantity { get; set; }
	public int price { get; set; }
}
