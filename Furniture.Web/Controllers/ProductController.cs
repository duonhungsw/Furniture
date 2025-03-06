namespace Furniture.Web.Controllers;

public class ProductController(IProductApi _productApi) : Controller
{
    public IActionResult ViewProducts()
	{
		return View();
	}
	public IActionResult ProductDetail()
	{
		return View();
	}
    public async Task<IActionResult> ProductHome(int pageIndex = 1)
    {
        var result = await _productApi.GetProductsAsync(pageIndex, 8);
		return View(result);
    }
}
