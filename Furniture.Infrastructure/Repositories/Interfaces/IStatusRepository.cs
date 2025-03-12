namespace Furniture.Infrastructure;

public interface IStatusRepository : IGenericRepository<Status>
{
	Task<Status?> GetStatusByNameAsync(string statusName);
}
