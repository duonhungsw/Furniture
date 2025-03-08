namespace Furniture.Web.Models;

public class CheckoutViewModel
{
	public List<OrderCheckout> Orders { get; set; } = new List<OrderCheckout>();
	public CreateOrderDto CreateOrder { get; set; } = new CreateOrderDto();
}
