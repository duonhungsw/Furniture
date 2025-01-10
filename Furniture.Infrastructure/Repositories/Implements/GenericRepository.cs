using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure.Repositories.Implements;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
	protected readonly ApplicationDbContext appDbContext;

	public GenericRepository(ApplicationDbContext context)
	{
		appDbContext = context;
	}
	public void Create(T entity)
	{
		appDbContext.Set<T>().Add(entity);
	}

	public void Delete(T entity)
	{
		appDbContext.Set<T>().Remove(entity);
	}

	public bool Exist(Guid id)
	{
		return appDbContext.Set<T>().Any(x => x.Id == id);
	}

	public async Task<IReadOnlyList<T>> GetAllAsync()
	{
		return await appDbContext.Set<T>().ToListAsync();
	}

	public async Task<T?> GetByIdAsync(Guid id)
	{
		return await appDbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<bool> SaveChangesAsync()
	{
		return await appDbContext.SaveChangesAsync() > 0;
	}

	public void Update(T entity)
	{
		appDbContext.Set<T>().Attach(entity);
		appDbContext.Entry(entity).State = EntityState.Modified;
	}
}
