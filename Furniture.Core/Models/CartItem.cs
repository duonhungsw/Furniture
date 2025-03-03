namespace Furniture.Core;

public class CartItem : BaseEntity
{
	[ForeignKey("Cart")]
	public Guid CartId { get; set; }
	public Cart? Cart { get; set; }

	[ForeignKey("Product")]
	public Guid ProductId { get; set; }
	public Product? Product { get; set; }

	[Required]
	public int Quantity { get; set; }

	[Required]
	public decimal Price { get; set; }

	[Required]
	public decimal TotalMoney { get; set; }

	[Required]
	public bool Status { get; set; }
}
