using Furniture.Core.Dtos.Order;
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
		if(orders.Count() == 0)
		{
			return RedirectToAction("ShoppingCart", "Cart");
		}
		var viewModel = new CheckoutViewModel
		{
			Orders = orders
		};
		HttpContext.Session.SetString("TotalMoney", viewModel.Orders.Sum(x => x.Price).ToString());
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
		var totalMoney = HttpContext.Session.GetString("TotalMoney");

		var paymentModel = new PaymentInformationModel
		{
			Name = "Hung",
			Amount = double.Parse(totalMoney!),
			OrderDescription = "Furniture Shop payment with VnPay",
			OrderType = 25000.ToString()
		};


		string paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);

		HttpContext.Session.SetObject("CheckoutData", model);
		HttpContext.Session.Remove("TotalMoney");

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
	public async Task<IActionResult> MonthlyRevenue(int? selectedYear)
	{
		int year = selectedYear ?? DateTime.Now.Year;

		var monthlyRevenue = await _api.GetMonthlyRevenue();

		var allMonths = new string[12] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
		var revenues = new double[12];

		if (monthlyRevenue != null && monthlyRevenue.MonthlyRevenue.Any())
		{
			var filteredData = monthlyRevenue.MonthlyRevenue
				.Where(x => x.Year == year)
				.ToList();

			if (filteredData.Any())
			{
				foreach (var item in filteredData)
				{
					var monthIndex = item.Month - 1;
					revenues[monthIndex] = (double)item.TotalRevenue;
				}
			}
		}

		var chartData = new MonthlyRevenueViewModel
		{
			MonthlyRevenue = monthlyRevenue?.MonthlyRevenue ?? new List<MonthlyRevenueDto>(),
			Labels = new List<string>(allMonths),
			Revenues = new List<double>(revenues),
			SelectedYear = year
		};
		return View(chartData);
	}
	[HttpGet]
	public async Task<IActionResult> GetOrders(int pageIndex = 1, int pageSize = 5)
	{
		var queryInfo = new QueryInfo
		{
			PageIndex = pageIndex,
			PageSize = pageSize,

		};
		var response = await _api.GetAllOrdersAsync(queryInfo);
		return View(response);
	}
	[HttpPost]
	public async Task<IActionResult> UpdateStatus(Guid orderId, Guid statusId)
	{
		var response = await _api.UpdateOrderStatus(orderId, statusId);
		if (response == null)
			return RedirectToAction("GetOrders");

		ModelState.AddModelError("", "Failed to update order status.");
		return RedirectToAction("GetOrders");
	}
}
