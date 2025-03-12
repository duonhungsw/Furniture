namespace Furniture.Web.Models;

public class PurchaseViewModel
{
	public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
	public List<StatusDto> Statuses { get; set; } = new List<StatusDto>();
	public Guid? SelectedStatusId { get; set; }
}
