namespace Furniture.Web.Controllers;

public class ProductController(IProductApi _productApi) : Controller
{
    public IActionResult ViewProducts()
	{
		return View();
	}
	public async Task<IActionResult> ProductDetail(Guid id)
	{
		var product = await _productApi.GetProductById(id);
		return View(product);
	}
    public async Task<IActionResult> ProductHome(int pageIndex = 1)
    {
        var result = await _productApi.GetProductsAsync(pageIndex);
		return View(result);
    }
	public async Task<IActionResult> SearchProducts([FromQuery] QueryInfo queryInfo)
	{
		var result = await _productApi.SearchProductsAsync(queryInfo);
		ViewBag.SearchText = queryInfo.SearchText;
		return View("ProductHome", result);
	}
}
