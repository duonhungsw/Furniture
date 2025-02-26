using Furniture.Core.Dtos.Order;

namespace Furniture.Service.Services.Implements;

public class OrderService(
	IOrderRepository _repository,
	IOrderItemRepository _orderItemRepository,
	IMapper _mapper) : IOrderService
{
	public async Task<bool> CreateOrderAsync(CreateOrderDto model)
	{
		var order = _mapper.Map<Order>(model);

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
}
