namespace Furniture.Core;

public class OrderDto
{
	public Guid Id { get; set; }
	public Guid AccountId { get; set; }
	public AccountDto? Account { get; set; }
	public string? Address { get; set; }
	public string? CreateAt { get; set; }
	public string? Phone { get; set; }
	public string? Note { get; set; }
	public decimal TotalMoney { get; set; }
	public string? PaymentMethod { get; set; }
	public Guid StatusId { get; set; }
	public StatusDto? Status { get; set; }

	public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
}
