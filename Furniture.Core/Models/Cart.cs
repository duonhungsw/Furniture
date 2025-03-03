namespace Furniture.Core;

public class Cart : BaseEntity
{
	[ForeignKey("User")]
	public Guid AccountId { get; set; }
	public Account? User { get; set; }

	public decimal CartTotal { get; set; }

	public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
