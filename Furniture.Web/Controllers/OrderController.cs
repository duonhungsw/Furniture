using Furniture.Web.Models;
using Furniture.Web.Services.VnPay;

namespace Furniture.Web.Controllers;

public class OrderController(
	IOrderApi _api,
	IAccountApi _accountApi,
	IStatusApi _statusApi,
	IVnPayService _vnPayService) : Controller
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
	[HttpGet]
	public async Task<ActionResult<PurchaseViewModel>> Purchase(Guid? statusId, QueryInfo queryInfo)
	{
		var account = await _accountApi.GetUserInfoAsync();
		if (account?.Content == null)
			return RedirectToAction("Index", "Home");

		var statuses = await _statusApi.GetStatuses();
		var orders = await _api.GetPurchases(account.Content.Id, statusId, queryInfo);

		var result = new PurchaseViewModel
		{
			Orders = orders,
			Statuses = statuses,
			SelectedStatusId = statusId
		};
		return View(result);
	}

	[HttpPost]
	public IActionResult VnPayCheckout(CheckoutViewModel model)
	{
		var paymentModel = new PaymentInformationModel
		{
			Name = "Hung",
			Amount = 48000,
			OrderDescription = "Furniture Shop payment with VnPay",
			OrderType = 25000.ToString()
		};


		string paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);

		HttpContext.Session.SetObject("CheckoutData", model);

		return Redirect(paymentUrl);
	}
	[HttpGet]
	public async Task<IActionResult> PaymentCallbackVnpay()
	{
		var response = _vnPayService.PaymentExecute(Request.Query);

		var model = HttpContext.Session.GetObject<CheckoutViewModel>("CheckoutData");
		var account = await _accountApi.GetUserInfoAsync();
		model!.CreateOrder.AccountId = account!.Content!.Id;

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
}
