using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure.Repositories.Implements;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
	public AccountRepository(ApplicationDbContext context) : base(context)
	{
	}
	public async Task<Account?> LoginAsync(Account account)
	{
		return await appDbContext.Accounts
					.FirstOrDefaultAsync(x =>
					(x.Name.Equals(account!.Name) ||
					x.Email.Equals(account.Email)) &&
					x.HashPassword == account.HashPassword);
	}
}
