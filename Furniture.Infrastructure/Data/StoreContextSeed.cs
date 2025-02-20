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
        if (!dbContext.Accounts.Any())
        {
            dbContext.Accounts.AddRange(ListAccount);
            await dbContext.SaveChangesAsync();
        }
    }
    public static List<Account> ListAccount => new List<Account>
    {
        new Account
    {
        Id = Guid.NewGuid(),
        Name = "Hung",
        Email = "ddhung2003@gmail.com",
        HashPassword = PasswordHasher.HashPasswordPBKDF2("Test@gmail.com"),
        Avatar = "null",
        BirthDay = "null",
        Phone = "123456789",
        RoleName = "Customer"
    },
    new Account
    {
        Id = Guid.NewGuid(),
        Name = "Employee",
        Email = "employee@gmail.com",
        HashPassword = PasswordHasher.HashPasswordPBKDF2("Employee@gmail.com"),
        Avatar = "null",
        BirthDay = "null",
        Phone = "null",
        RoleName = "Employee"
    },
    new Account
    {
        Id = Guid.NewGuid(),
        Name = "Admin",
        Email = "admin@gmail.com",
        HashPassword = PasswordHasher.HashPasswordPBKDF2("Admin@gmail.com"),
        Avatar = "null",
        BirthDay = "null",
        Phone = "null",
        RoleName = "Admin"
    }
    };
}
