namespace Furniture.Web.Controllers;

public class AdminController(IProductApi _productApi,
    IAccountApi _accountApi) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> ProductList([FromQuery] QueryInfo queryInfo)
    {
        var result = await _productApi.GetProductsAsync(queryInfo);
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
    public async Task<IActionResult> Create([FromForm] ProductDto model)
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

         
            formData.Add(new StringContent(DateTime.Now.ToString("o")), "CreatedAt"); 

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
    public async Task<IActionResult> Update([FromForm] ProductDto model)
    {
        var existingProduct = await _productApi.GetProductById(model.Id);
        if (existingProduct == null)
        {
            ModelState.AddModelError("", "Product not found.");
            return View(model);
        }

    
        model.Name ??= existingProduct.Name;
        model.Description ??= existingProduct.Description;
        model.Price = model.Price == 0 ? existingProduct.Price : model.Price;
        model.Type ??= existingProduct.Type;
        model.Brand ??= existingProduct.Brand;
        model.QuantityInStock = model.QuantityInStock == 0 ? existingProduct.QuantityInStock : model.QuantityInStock;
        model.PictureUrl ??= existingProduct.PictureUrl;

        bool hasNoOldImage = string.IsNullOrEmpty(existingProduct.PictureUrl);
        bool hasNoNewImage = model.Images == null || model.Images.Count == 0;

        if (hasNoOldImage && hasNoNewImage)
        {
            ModelState.AddModelError("Images", "Please upload at least one image.");
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
            hasNoNewImage;

        if (isSameData)
        {
            return RedirectToAction("ProductList", "Admin");
        }

        using (var formData = new MultipartFormDataContent())
        {
            formData.Add(new StringContent(model.Id.ToString()), "Id");
            formData.Add(new StringContent(model.Name), "Name");
            formData.Add(new StringContent(model.Description), "Description");
            formData.Add(new StringContent(model.Price.ToString()), "Price");
            formData.Add(new StringContent(model.Type), "Type");
            formData.Add(new StringContent(model.Brand), "Brand");
            formData.Add(new StringContent(model.QuantityInStock.ToString()), "QuantityInStock");
            formData.Add(new StringContent(model.PictureUrl), "PictureUrl");
            formData.Add(new StringContent(DateTime.Now.ToString("o")), "LastModified");

            if (!hasNoNewImage)
            {
                foreach (var file in model.Images)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    formData.Add(streamContent, "Images", file.FileName);
                }
            }

            try
            {
                var result = await _productApi.Update(formData);
                if (result)
                {
                    TempData["SuccessMessage"] = "Product updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update product.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the product: " + ex.Message;
            }

            return RedirectToAction("ProductList", "Admin");
        }
    }


    public async Task<IActionResult> ConfirmDelete(Guid id)
    {
        var product = await _productApi.GetProductById(id);
        return View(product);
    }
    [HttpPost]
    public async Task<IActionResult> DeleteProduct([FromForm] Guid id)
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
