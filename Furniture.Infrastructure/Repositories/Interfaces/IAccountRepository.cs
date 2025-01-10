namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface IAccountRepository
{
	Task<Account?> LoginAsync(Account account);
}
