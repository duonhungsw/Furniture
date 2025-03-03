using System.ComponentModel.DataAnnotations;

namespace Furniture.Core;

public abstract class BaseEntity
{
	[Key]
	public Guid Id { get; set; } = Guid.NewGuid();
}
