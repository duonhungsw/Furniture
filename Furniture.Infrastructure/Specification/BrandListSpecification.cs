using Furniture.Core.Models;

namespace Furniture.Infrastructure.Specification;

public class BrandListSpecification : BaseSpecification<Product, string>
{
    public BrandListSpecification()
    {
        AddSelect(x => x.Brand);
        Distinct();
    }
}
