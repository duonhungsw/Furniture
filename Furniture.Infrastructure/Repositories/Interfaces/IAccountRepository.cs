using Furniture.Core.Dtos.Account;

namespace Furniture.Infrastructure.Repositories.Interfaces;

public interface IAccountRepository : IGenericRepository<Account>
{
	Task<Account?> LoginAsync(Account account);
	Task<Account?> GetByEmailAsync(string Email);
	Task<List<AccountDto>> GetAccountsAsync();
}
