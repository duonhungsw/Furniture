namespace Furniture.Core;

public class UpdateAccountDto
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public IFormFile? Avatar { get; set; }
	public string? BirthDay { get; set; }
	public string? Phone { get; set; }
}
