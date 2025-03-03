namespace Furniture.Core;

public class Order : BaseEntity
{
	[ForeignKey("Account")]
	public Guid AccountId { get; set; }
	public Account? AppUser { get; set; }

	[Required]
	public string? Country { get; set; }

	[Required]
	public string? City { get; set; }

	[Required]
	public string? District { get; set; }

	[Required]
	public string? Town { get; set; }

	[Required]
	public string? Detail { get; set; }

	[Required]
	public string? Phone { get; set; }

	[Required]
	public string? Note { get; set; }

	[Required]
	public decimal TotalMoney { get; set; }

	[Required]
	public string? PaymentMethod { get; set; }

	public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
