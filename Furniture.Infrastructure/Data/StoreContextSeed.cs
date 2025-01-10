using Furniture.Core.Models;
using System.Reflection;
using System.Text.Json;

namespace Furniture.Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {

        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!dbContext.Products.Any())
        {
			//E:\Project H\FurnitureShop\FurnitureShop\FurnitureShopServer\Furniture.Infrastructure\Data\SeedData
			var productData = await File
                .ReadAllTextAsync("../Furniture.Infrastructure/Data/SeedData/products.json");

            var product = JsonSerializer.Deserialize<List<Product>>(productData);

            if (product == null) return;
            dbContext.Products.AddRange(product);
            await dbContext.SaveChangesAsync();
        }
    }
}
