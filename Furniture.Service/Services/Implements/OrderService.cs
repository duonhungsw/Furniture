namespace Furniture.Service;

public class OrderService(
	IOrderRepository _repository,
	IOrderItemRepository _orderItemRepository,
	IStatusRepository _statusRepository,
	IMapper _mapper) : IOrderService
{

	public async Task<bool> CreateOrderAsync(CreateOrderDto model)
	{
		try
		{
			var order = _mapper.Map<Order>(model);
			order.Country = "Viet Nam";
			order.AccountId = model.AccountId;
			order.StatusId = (await _statusRepository.GetStatusByNameAsync(OrderStatus.Pending.ToString()))!.Id;
			order.TotalMoney = model.OrderItems.Sum(item => item.Quantity * item.Price);

			_repository.Create(order);

			foreach (var item in model.OrderItems)
			{
				var orderItem = new OrderItem
				{
					OrderId = order.Id, 
					ProductId = item.ProductId,
					Quantity = item.Quantity,
				};

				_orderItemRepository.Create(orderItem);
			}

			await _repository.SaveChangesAsync(); 

			return true;
		}
		catch 
		{
			return false;
		}
	}

	public async Task<bool> ChangeStatusAsync(Guid orderId, string roleName)
	{
		var order = await _repository.GetByIdAsync(orderId);
		if (order == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Order", orderId));

		if (roleName == AppRoles.Customer.ToString())
		{
			order!.StatusId = (await _statusRepository.GetStatusByNameAsync(OrderStatus.Cancelled.ToString()))!.Id;

			_repository.Update(order);
			return await _repository.SaveChangesAsync() ? true : false;
		}
		if(roleName == AppRoles.Admin.ToString())
		{
			order!.StatusId = (await _statusRepository.GetStatusByNameAsync(OrderStatus.Complete.ToString()))!.Id;

			_repository.Update(order);
			return await _repository.SaveChangesAsync() ? true : false;
		}
		return false;
	}
	public async Task<List<OrderCheckout>> GetOrdersForAccountAsync(Guid id)
	 => await _repository.GetOrdersForAccountAsync(id);

	public async Task<List<OrderDto>> GetOrdersAsync(Guid accountId, QueryInfo queryInfo, Guid statusId)
		=> await _repository.GetOrdersAsync(accountId, queryInfo, statusId);
}
