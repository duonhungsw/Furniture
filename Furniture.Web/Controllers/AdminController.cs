namespace Furniture.Web.Controllers;

public class AdminController(IProductApi _productApi) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> ProductList(int pageIndex = 1, int pageSize = 5)
    {
        var queryInfo = new QueryInfo { PageIndex = pageIndex, PageSize = pageSize };
        var pagedResult = await _productApi.GetProductsWithPaging(queryInfo);

        return View(pagedResult.Items);
    }
}
