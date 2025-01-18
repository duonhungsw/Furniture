namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface IAccountRepository : IGenericRepository<Account>
{
	Task<Account?> LoginAsync(Account account);
}
