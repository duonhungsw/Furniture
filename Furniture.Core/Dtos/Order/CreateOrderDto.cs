namespace Furniture.Core;
public class CreateOrderDto
{
	public Guid Id { get; set; }
	public Guid AccountId { get; set; }
	public string? Country { get; set; }
	public string? City { get; set; }
	public string? District { get; set; }
	public string? Town { get; set; }
	public string? Detail { get; set; }
	public string? Phone { get; set; }
	public string? Note { get; set; }
	public string? PaymentMethod { get; set; }
	public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
}
