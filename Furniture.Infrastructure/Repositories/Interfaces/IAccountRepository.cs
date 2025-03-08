namespace Furniture.Infrastructure;

public interface IAccountRepository : IGenericRepository<Account>
{
	Task<Account?> LoginAsync(Account account);
	Task<Account?> GetByEmailAsync(string Email);
	Task<List<AccountDto>> GetAccountsAsync();
	Task<bool> VerifyAccountByPasswordAsync(Guid id, string password);
}	
