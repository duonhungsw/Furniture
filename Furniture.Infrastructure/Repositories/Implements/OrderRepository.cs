namespace Furniture.Infrastructure.Repositories.Implements
{
	public class OrderRepository : GenericRepository<Order>, IOrderRepository
	{
		public OrderRepository(ApplicationDbContext context) : base(context)
		{

		}

	}
}
