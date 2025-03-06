namespace Furniture.Service;

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
		order.StatusId = (await _repository.GetStatusByNameAsync(OrderStatus.Pending.ToString()))!.Id;
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
			throw new UnauthorizedException();
		return await _repository.GetOrdersAsync(customer.Id);
	}
	public async Task<bool> ChangeStatusAsync(Guid orderId, string roleName)
	{
		var order = await _repository.GetByIdAsync(orderId);
		if (order == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Order", orderId));

		if (roleName == AppRoles.Customer.ToString())
		{
			order!.StatusId = (await _repository.GetStatusByNameAsync(OrderStatus.Cancelled.ToString()))!.Id;

			_repository.Update(order);
			return await _repository.SaveChangesAsync() ? true : false;
		}
		if(roleName == AppRoles.Admin.ToString())
		{
			order!.StatusId = (await _repository.GetStatusByNameAsync(OrderStatus.Complete.ToString()))!.Id;

			_repository.Update(order);
			return await _repository.SaveChangesAsync() ? true : false;
		}
		return false;
	}
	public async Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id)
	 => await _repository.GetOrdersForAccountAsync(id);
}
