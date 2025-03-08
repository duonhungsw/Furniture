

namespace Furniture.Infrastructure;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
	public AccountRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<List<AccountDto>> GetAccountsAsync()
	{
		return await appDbContext.Accounts
			.AsNoTracking()
			.Select(account => new AccountDto
			{
				Id = account.Id,
				Name = account.Name,
				Email = account.Email,
				Avatar = account.Avatar,
				BirthDay = account.BirthDay,
				Phone = account.Phone
			})
			.ToListAsync();
	}

	public async Task<Account?> GetByEmailAsync(string Email)
	{
		return await appDbContext.Accounts.FirstOrDefaultAsync(x => x.Email == Email);
	}

	public async Task<Account?> LoginAsync(Account account)
	{
		return await appDbContext.Accounts
					.FirstOrDefaultAsync(x =>
					(x.Name.Equals(account!.Name) ||
					x.Email.Equals(account.Email)) &&
					x.HashPassword == account.HashPassword);
	}

	public async Task<bool> VerifyAccountByPasswordAsync(Guid id, string password)
	{
		return await appDbContext.Accounts
			.AsNoTracking()
			.AnyAsync(x => x.Id == id && x.HashPassword.Equals(password));
	}
}
