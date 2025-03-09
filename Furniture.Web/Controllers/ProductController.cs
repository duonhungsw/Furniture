namespace Furniture.Web.Controllers;

public class ProductController(IProductApi _productApi) : Controller
{
    public IActionResult ViewProducts()
	{
		return View();
	}
	public async Task<IActionResult> ProductDetail(Guid id)
	{
		var product = await _productApi.GetProductByIdAsync(id);
		return View(product);
	}
    public async Task<IActionResult> ProductHome(int pageIndex = 1)
    {
        var result = await _productApi.GetProductsAsync(pageIndex);
		return View(result);
    }
}
