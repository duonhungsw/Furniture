using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Furniture.Core.Models;

public class OrderItem : BaseEntity
{
	[ForeignKey("Order")]
	public Guid OrderId { get; set; }
	public Order? Order { get; set; }

	[ForeignKey("Product")]
	public Guid ProductId { get; set; }
	public Product? Product { get; set; }

	[Required]
	public int Quantity { get; set; }
}
