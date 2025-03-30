using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure;


public class ProductRepository : GenericRepository<Product>, IProductRepository
{
	public ProductRepository(ApplicationDbContext context) : base(context)
	{

	}

    public async Task<List<Product>> FilterProductsAsync(FilterProductInfo filterInfo)
    {
        var query = appDbContext.Products.AsQueryable();

        if (filterInfo.Brands != null && filterInfo.Brands.Any())
        {
            query = query.Where(p => filterInfo.Brands.Contains(p.Brand.ToLower()));
        }

        if (filterInfo.Types != null && filterInfo.Types.Any())
        {
            query = query.Where(p => filterInfo.Types.Contains(p.Type.ToLower()));
        }

        if (!string.IsNullOrEmpty(filterInfo.SearchText))
        {
            query = query.Where(p => p.Name.Contains(filterInfo.SearchText));
        }

		if (!string.IsNullOrEmpty(filterInfo.OrderBy))
		{
			var orderParams = filterInfo.OrderBy.Split(':');
			var orderByField = orderParams[0];  // Price
			var orderDirection = orderParams.Length > 1 ? orderParams[1] : "Asc";  

			if (orderByField.Equals("Price", StringComparison.OrdinalIgnoreCase))
			{
				if (orderDirection.Equals("Asc", StringComparison.OrdinalIgnoreCase))
				{
					query = query.OrderBy(p => p.Price);
				}
				else if (orderDirection.Equals("Desc", StringComparison.OrdinalIgnoreCase))
				{
					query = query.OrderByDescending(p => p.Price);
				}
			}
		}

		return await query.ToListAsync();
    }

    public async Task<List<string>> GetBrandAsync()
	{
		return await appDbContext.Products
			.Select(p => p.Brand)
			.Distinct()
			.ToListAsync();
	}

	public async Task<List<string>> GetTypeAsync()
	{
		return await appDbContext.Products
			.Select(p => p.Type)
			.Distinct()
			.ToListAsync();
	}

	public async Task<List<Product>> ListProductAsync()
	{
		return await appDbContext.Products.ToListAsync();
	}

	public async Task<List<Product>> SearchProductsAsync(string keyword)
	{
		return await appDbContext.Products
					.AsNoTracking()
					.Where(p => EF.Functions.Collate(p.Name, "Latin1_General_CI_AI").Contains(keyword)
						&& p.QuantityInStock > 0).ToListAsync();
	}
    //public async Task<Product?> FindByIdAsync(Guid id)
    //{

    //    var product =  await appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    //    if (product == null)
    //    {
    //        return null;
    //    }
    //    return product;
    //}
    public async Task<bool> IsImageUsedByOtherProductsAsync(string imageUrl, Guid productId)
    {
        return await appDbContext.Products
            .AnyAsync(p => p.Id != productId && p.PictureUrl.Contains(imageUrl));
    }
}