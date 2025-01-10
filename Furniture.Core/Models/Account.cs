namespace Furniture.Core.Models;

public class Account : BaseEntity
{
	public string Name { get;  set; } = default!;
	public string Email { get;  set; } = default!;
	public string HashPassword { get; set; } = default!;
	public string? Avatar { get; set; }
	public string? BirthDay { get;  set; }
	public string? Phone { get;  set; }
	public string RoleName { get; set; } = default!;
}
