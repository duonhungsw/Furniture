using Furniture.Core.Dtos.Product;

namespace Furniture.Core.Dtos.Order
{
	public class OrderItemDto
	{
		public Guid Id { get; set; }
		public Guid OrderId { get; set; }
		public Guid ProductId { get; set; }
		public OrderDto? Order { get; set; }
		public ProductDto? Product { get; set; }
		public int Quantity { get; set; }
	}
}
