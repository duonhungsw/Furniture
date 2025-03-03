using Furniture.Common;
using Furniture.Common.Exceptions;
using Furniture.Core.Dtos.Order;

namespace Furniture.Service.Services.Implements;

public class OrderService(
	IOrderRepository _repository,
	IOrderItemRepository _orderItemRepository,
	ITokenService _tokenService,
	IMapper _mapper) : IOrderService
{
	public async Task<bool> CreateOrderAsync(CreateOrderDto model)
	{
		var account = await _tokenService.Authenticate();

		if (account == null)
			throw new UnauthorizedAccessException();

		var order = _mapper.Map<Order>(model);
		order.AccountId = account.Id;
		order.TotalMoney = model.OrderItems.Sum(item => item.Quantity * item.price);
		_repository.Create(order);
		if (await _repository.SaveChangesAsync())
		{
			var orderItems = model.OrderItems.Select(item => new OrderItem
			{
				OrderId = order.Id,
				ProductId = item.ProductId,
				Quantity = item.Quantity,
			}).ToList();

			await _orderItemRepository.AddRangeAsync(orderItems);
			return await _orderItemRepository.SaveChangesAsync();
		}

		return false;
	}

	public async Task<List<OrderItemDto>> GetOrdersAsync()
	{
		var customer = await _tokenService.Authenticate();
		if (customer == null)
			throw new NotFoundException("Unauthentication");
		return await _repository.GetOrdersAsync(customer.Id);
	}
}
