namespace Furniture.Web.Controllers;

public class AdminController(IProductApi _productApi,
    IAccountApi _accountApi) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> ProductList(QueryInfo queryInfo)
    {
        var result = await _productApi.GetProductsAsync(queryInfo.PageIndex);
        ViewBag.PageIndex = queryInfo.PageIndex;
        ViewBag.PageSize = queryInfo.PageSize;
        ViewBag.SearchText = queryInfo.SearchText;
        ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / queryInfo.PageSize);

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
            ModelState.AddModelError("Images", "Choose at least 1 picture.");
            return View(model);
        }
        using (var formData = new MultipartFormDataContent())
        {
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
                TempData["SuccessMessage"] = "Product created successfully!";
                TempData.Keep("SuccessMessage");
                return RedirectToAction("ProductList");
            }
        }
        ModelState.AddModelError("", "An error occurred while creating the product.");
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
        var existingProduct = await _productApi.GetProductById(model.Id);
        if (existingProduct == null)
        {
            ModelState.AddModelError("", "Product not found.");
            return View(model);
        }
        bool isSameData =
            existingProduct.Name == model.Name &&
            existingProduct.Description == model.Description &&
            existingProduct.Price == model.Price &&
            existingProduct.Type == model.Type &&
            existingProduct.Brand == model.Brand &&
            existingProduct.QuantityInStock == model.QuantityInStock &&
            existingProduct.PictureUrl == model.PictureUrl &&
            (model.Images == null || model.Images.Count == 0);
        if (isSameData)
        {
            return RedirectToAction("ProductList");
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
            else
            {
                formData.Add(new StringContent(existingProduct.PictureUrl ?? ""), "PictureUrl");
            }
            var result = await _productApi.Update(formData);
            if (result)
            {
                TempData["SuccessMessage"] = "Product updated successfully!";
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
        {
            TempData["SuccessMessage"] = "Product deleted successfully!";
        }
        return RedirectToAction("ProductList");
    }
    [HttpGet]
    public async Task<IActionResult> ManageAccount(QueryInfo queryInfo)
    {
        var result = await _accountApi.GetAccounts(queryInfo);
        ViewBag.PageIndex = queryInfo.PageIndex;
        ViewBag.PageSize = queryInfo.PageSize;
        ViewBag.SearchText = queryInfo.SearchText;
        ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / queryInfo.PageSize);
        return View(result.Items);
    }
    [HttpGet]
    public async Task<IActionResult> UpdateRole(Guid id)
    {
        var product = await _accountApi.GetAccountByIdAsync(id);
        return View(product);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateRole([FromForm] AccountDto model)
    {
        var result = await _accountApi.UpdateRole(model);
        if (result)
        {
            TempData["SuccessMessage"] = "AccountRole Updated successfully!";
        }
        return RedirectToAction("ManageAccount");
    }
}
