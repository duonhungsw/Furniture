using Furniture.Core.Dtos.Order;

namespace Furniture.Service;

public class OrderService(
	IOrderRepository _repository,
	IOrderItemRepository _orderItemRepository,
	IStatusRepository _statusRepository,
	ICartItemRepository _cartItemRepository,
	ICartRepository _cartRepository,
	IProductRepository _productRepository,
	IMapper _mapper) : IOrderService
{

	public async Task<bool> CreateOrderAsync(CreateOrderDto model)
	{
		using var transaction = await _repository.BeginTransactionAsync(); 

		try
		{
			var status = await _statusRepository.GetStatusByNameAsync(OrderStatus.Pending.ToString());
			if (status == null) return false; 

			var order = _mapper.Map<Order>(model);
			order.Country = "Viet Nam";
			order.AccountId = model.AccountId;
			order.StatusId = status.Id;
			order.TotalMoney = model.OrderItems.Sum(item => item.Quantity * item.Price);

			_repository.Create(order);
			//await _repository.SaveChangesAsync();

			//var orderItems = model.OrderItems.Select(item => new OrderItem
			//{
			//	OrderId = order.Id,
			//	ProductId = item.ProductId,
			//	Quantity = item.Quantity
			//}).ToList();

			//await _orderItemRepository.AddRangeAsync(orderItems); 
			await _repository.SaveChangesAsync();

			//foreach (var items in orderItems)
			//{
			//	_orderItemRepository.Delete(items);
			//}

			// Delete cart items was bought
			var cartOfAccount = await _cartRepository.GetCartByAccountIdAsync(model.AccountId);

			var cartItemsToDelete = new List<CartItem>();

			foreach (var product in model.OrderItems)
			{
				var cartItem = await _cartItemRepository.GetCartsItemByCartIdAndProductIdAsync(cartOfAccount!.Id, product.ProductId);
				foreach (var item in cartItem!)
				{
					// update quantityOfStock product
					var productCartItem = await _productRepository.GetByIdAsync(item.ProductId);
					productCartItem!.QuantityInStock -= item.Quantity;
					_productRepository.Update(productCartItem);

					 _cartItemRepository.Delete(item);
				}
			}

			//if (cartItemsToDelete.Any())
			//{
			//	_cartItemRepository.DeleteRangeAsync(cartItemsToDelete);
			//	await _repository.SaveChangesAsync();
			//}
			await _repository.SaveChangesAsync();
			await transaction.CommitAsync(); 
			return true;
		}
		catch
		{
			await transaction.RollbackAsync();
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
