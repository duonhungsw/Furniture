using Furniture.Core.Dtos.Product;
using Microsoft.EntityFrameworkCore;

namespace Furniture.Infrastructure.Repositories.Implements;


public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {

    }
    public async Task<List<Product>> ListProductAsync()
    {
        return await appDbContext.Products.ToListAsync();
    }
}