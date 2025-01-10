using System.ComponentModel.DataAnnotations;

namespace Furniture.Core.Models;

public abstract class BaseEntity
{
	[Key]
	public Guid Id { get; set; } = Guid.NewGuid();
}
