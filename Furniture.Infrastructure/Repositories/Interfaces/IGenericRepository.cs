namespace Furniture.Infrastructure;

public interface IGenericRepository<T> where T : BaseEntity
{
	Task<T?> GetByIdAsync(Guid id);
	Task<IReadOnlyList<T>> GetAllAsync();
	void Delete(T entity);
	void Update(T entity);
	void Create(T entity);
	bool Exist(Guid id);
	Task<bool> SaveChangesAsync();
}
