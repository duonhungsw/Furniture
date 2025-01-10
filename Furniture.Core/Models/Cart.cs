using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Furniture.Core.Models;

public class Cart : BaseEntity
{
	[ForeignKey("User")]
	public Guid AccountId { get; set; }
	public Account? User { get; set; }

	public decimal CartTotal { get; set; }

	public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
