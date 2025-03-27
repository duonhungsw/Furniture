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
		var brands = await _productApi.GetProductsBrand();
		var types = await _productApi.GetProductsType();

		ViewBag.Brands = brands;
		ViewBag.Types = types;

		return View(result);
    }
	public async Task<IActionResult> SearchProducts([FromQuery] QueryInfo queryInfo)
	{
		var result = await _productApi.SearchProductsAsync(queryInfo);
		var brands = await _productApi.GetProductsBrand();
		var types = await _productApi.GetProductsType();

		ViewBag.Brands = brands;
		ViewBag.Types = types;
		ViewBag.SearchText = queryInfo.SearchText;
		return View("ProductHome", result);
	}

    public async Task<IActionResult> FilterProducts([FromBody] FilterProductInfo filterInfo)
    {
        var result = await _productApi.FilterProductsAsync(filterInfo);
        return Json(result);
    }
}
