namespace Furniture.Web.Controllers;

public class AdminController(IProductApi _productApi) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> ProductList(int pageIndex = 1, int pageSize = 5)
    {
        var result = await _productApi.GetProductsAsync(pageIndex);

        ViewBag.PageIndex = pageIndex;
        ViewBag.TotalPages = result.TotalPages;

        return View(result.Items);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(ProductDto model)
    {
        if (model.Images == null || model.Images.Count == 0)
        {
            ModelState.AddModelError("Images", "Vui lòng chọn ít nhất một ảnh.");
            return View(model);
        }
        using (var formData = new MultipartFormDataContent())
        {
            formData.Add(new StringContent(model.Id.ToString()), "Id");
            formData.Add(new StringContent(model.Name ?? ""), "Name");
            formData.Add(new StringContent(model.Description ?? ""), "Description");
            formData.Add(new StringContent(model.Price.ToString()), "Price");
            formData.Add(new StringContent(model.Type ?? ""), "Type");
            formData.Add(new StringContent(model.Brand ?? ""), "Brand");
            formData.Add(new StringContent(model.QuantityInStock.ToString()), "QuantityInStock");
            foreach (var file in model.Images)
            {
                var streamContent = new StreamContent(file.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                formData.Add(streamContent, "Images", file.FileName);
            }
            var result = await _productApi.Create(formData);
            if (result)
            {
                return RedirectToAction("ProductList");
            }
        }
        ModelState.AddModelError("", "Có lỗi xảy ra khi tạo sản phẩm.");
        return View(model);
    }
    public async Task<IActionResult> Update(Guid id)
    {
        var product = await _productApi.GetProductById(id);
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Update(ProductDto model)
    {
        using (var formData = new MultipartFormDataContent())
        {
            formData.Add(new StringContent(model.Id.ToString()), "Id");
            formData.Add(new StringContent(model.Name ?? ""), "Name");
            formData.Add(new StringContent(model.Description ?? ""), "Description");
            formData.Add(new StringContent(model.Price.ToString()), "Price");
            formData.Add(new StringContent(model.Type ?? ""), "Type");
            formData.Add(new StringContent(model.Brand ?? ""), "Brand");
            formData.Add(new StringContent(model.QuantityInStock.ToString()), "QuantityInStock");
            if (!string.IsNullOrEmpty(model.PictureUrl))
            {
                formData.Add(new StringContent(model.PictureUrl), "PictureUrl");
            }

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var file in model.Images)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    formData.Add(streamContent, "Images", file.FileName);
                }
            }

            var result = await _productApi.Update(formData);
            if (result)
            {
                return RedirectToAction("ProductList");
            }
        }
        return View(model);
    }
    public async Task<IActionResult> ConfirmDelete(Guid id)
    {
        var product = await _productApi.GetProductById(id);
        return View(product);
    }
    [HttpPost]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        bool result = await _productApi.Delete(id);
        if (result)
            return RedirectToAction("ProductList");

        ModelState.AddModelError("", "Delete Failed.");
        return RedirectToAction("ProductList");
    }
}
