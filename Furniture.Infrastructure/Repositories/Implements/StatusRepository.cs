namespace Furniture.Infrastructure;

public class StatusRepository : GenericRepository<Status>, IStatusRepository
{
	public StatusRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<Status?> GetStatusByNameAsync(string statusName)
		=> await appDbContext.Statuses.FirstOrDefaultAsync(x => x.Name == statusName);
}
