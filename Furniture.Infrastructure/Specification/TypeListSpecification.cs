using Furniture.Core.Models;

namespace Furniture.Infrastructure.Specification;

public class TypeListSpecification : BaseSpecification<Product, string>
{
    public TypeListSpecification()
    {
        AddSelect(x => x.Type);
        Distinct();
    }
}
