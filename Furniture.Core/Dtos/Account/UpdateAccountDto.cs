using Microsoft.AspNetCore.Http;

namespace Furniture.Core;

public class UpdateAccountDto
{
	public Guid Id { get; set; }
	public string? Name { get; set; } 
	public IFormFile? Avatar { get; set; }
	public DateTime? BirthDay { get; set; }
	public string? Phone { get; set; }
}
