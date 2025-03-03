namespace Furniture.Infrastructure;

public class BrandListSpecification : BaseSpecification<Product, string>
{
	public BrandListSpecification()
	{
		AddSelect(x => x.Brand);
		Distinct();
	}
}
