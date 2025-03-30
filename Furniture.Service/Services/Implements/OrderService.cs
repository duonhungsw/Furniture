using Furniture.Core.Dtos.Order;

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
    public async Task<MonthlyRevenueViewModel> GetMonthlyRevenue()
    {
        var revenueList = await _repository.GetMonthlyRevenue();

        return new MonthlyRevenueViewModel
        {
            MonthlyRevenue = revenueList,
            Labels = revenueList.Select(x => $"{x.Month}/{x.Year}").ToList(),
            Revenues = revenueList.Select(x => (double)x.TotalRevenue).ToList()
        };
    }
    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _repository.GetAllOrdersAsync();

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            AccountId = order.AccountId,
            Account = order.Account != null
                ? new AccountDto
                {
                    Id = order.Account.Id,
                    Name = order.Account.Name,
                    Email = order.Account.Email
                }
                : null,
            Address = order.Address,
            CreateAt = order.CreateAt ?? "",

            Phone = order.Phone,
            Note = order.Note,
            TotalMoney = order.TotalMoney,
            PaymentMethod = order.PaymentMethod,
            StatusId = order.StatusId,
            Status = order.Status != null
                ? new StatusDto
                {
                    Id = order.Status.Id,
                    Name = order.Status.Name
                }
                : null,
            OrderItems = order.OrderItems.Select(item => new CreateOrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price,
                Product = item.Product != null
                    ? new ProductDto
                    {
                        Id = item.Product.Id,
                        Name = item.Product.Name
                    }
                    : null
            }).ToList()
        }).ToList();
    }
    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid statusId)
    {
        return await _repository.UpdateOrderStatusAsync(orderId, statusId);
    }
}
