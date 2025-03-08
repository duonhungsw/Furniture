using Furniture.Web.Models;

namespace Furniture.Web.Controllers;

public class OrderController(
	IOrderApi _api,
	IAccountApi _accountApi) : Controller
{
	[HttpGet]
	public async Task<ActionResult<List<OrderCheckout>>> Checkout()
	{
		var account = await _accountApi.GetUserInfoAsync();
		var orders = await _api.GetOrdersForAccount(account!.Content!.Id);
		var viewModel = new CheckoutViewModel
		{
			Orders = orders
		};
		HttpContext.Session.SetObject("OrderSession", orders);

		return View(viewModel);
	}
	[HttpPost]
	public async Task<IActionResult> Checkout([FromForm] CheckoutViewModel model)
	{
		var account = await _accountApi.GetUserInfoAsync();
		model.CreateOrder.AccountId = account!.Content!.Id;

		var order = HttpContext.Session.GetObject<List<OrderCheckout>>("OrderSession");

		if (order != null && order.Any())
		{
			model.CreateOrder.OrderItems = order.Select(item => new CreateOrderItemDto
			{
				ProductId = item.ProductId,
				Quantity = item.Quantity,
				Price = item.Price
			}).ToList();
		}
		else
		{
			ModelState.AddModelError("", "No items found in order.");
			return View(model);
		}

		var result = await _api.CreateOrder(model.CreateOrder);
		if (result)
		{
			return RedirectToAction("ThankYou"); 
		}
		else
		{
			ModelState.AddModelError("", "Order creation failed.");
			return View(model);
		}
	}
	[HttpGet]
	public IActionResult ThankYou()
	{
		return View();
	}
}
