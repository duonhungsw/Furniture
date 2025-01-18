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
    public static List<Account> ListAccount => new List<Account>
    {
        new Account
    {
        Id = Guid.NewGuid(),
        Name = "Hung",
        Email = "ddhung2003@example.com",
        HashPassword = PasswordHasher.HashPasswordPBKDF2("Test@gmail.com"),
        Avatar = "null",
        BirthDay = "null",
        Phone = "123456789",
        RoleName = "Customer"
    },
    new Account
    {
        Id = Guid.NewGuid(),
        Name = "Jane Smith",
        Email = "janesmith@example.com",
        HashPassword = "hashed_password_456",
        Avatar = "avatar2.png",
        BirthDay = "1992-02-02",
        Phone = "987654321",
        RoleName = "User"
    }
    };
}
