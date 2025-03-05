namespace Furniture.Core;

public class ChangePhoneNumberDto
{
	public Guid Id { get; set; }
	public string? Phone { get; set; }
	public string? session { get; set; }
}
