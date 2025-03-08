namespace Furniture.Core;

public class AccountActionDto
{
	public Guid Id { get; set; }
	public string? Password { get; set; }
	public string? Action { get; set; }
	public string? NewPhoneNumber { get; set; }
	public string? NewPassword { get; set; }
	public string? NewEmail { get; set; }
}
