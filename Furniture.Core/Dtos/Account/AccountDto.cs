namespace Furniture.Core.Dtos.Account;

public class AccountDto
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public string? Avatar { get; set; }
	public string? BirthDay { get; set; }
	public string? Phone { get; set; }
	public string? RoleName { get; set; }
}
