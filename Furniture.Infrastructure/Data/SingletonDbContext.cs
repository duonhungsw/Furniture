namespace Furniture.Infrastructure.Data;

public class SingletonDbContext<T> where T : class, new()
{
	private static T? instance;
	private static readonly object _lock = new object();
	public static ApplicationDbContext appDbContext = new ApplicationDbContext();

	public static T Instance
	{
		get
		{
			lock (_lock)
			{
				if (instance == null)
				{
					instance = new T();
				}
				return instance;
			}
		}
	}

}
