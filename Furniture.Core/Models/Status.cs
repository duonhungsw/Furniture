namespace Furniture.Core;

public class Status : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public ICollection<Order> Orders { get; set; } = new List<Order>();
}
