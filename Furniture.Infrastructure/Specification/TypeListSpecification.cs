namespace Furniture.Infrastructure;

public class TypeListSpecification : BaseSpecification<Product, string>
{
	public TypeListSpecification()
	{
		AddSelect(x => x.Type);
		Distinct();
	}
}
