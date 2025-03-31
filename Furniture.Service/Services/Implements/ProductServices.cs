namespace Furniture.Service;
public class ProductServices(
	IProductRepository _repository,
	IFileStorageService _storageService,
    IOrderRepository _orderRepository,
    ICartRepository _cartRepository,
    IMapper _mapper) : IProductServices
{
    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Product", id));
        bool isUsedInOrders = await _orderRepository.IsProductUsedInOrdersAsync(id);
        bool isUsedInCart = await _cartRepository.IsProductUsedInCartAsync(id);

        if (isUsedInOrders || isUsedInCart)
        {
            throw new InvalidOperationException("Existing this product in order or cart.");
        }

        string containerName = ContainerName.product.ToString();
        if (!string.IsNullOrEmpty(product.PictureUrl))
        {
            var imageUrls = product.PictureUrl.Split(',');
            foreach (var imageUrl in imageUrls)
            {
                if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uri))
                {
                    string blobName = Path.GetFileName(uri.LocalPath);
                    bool isImageShared = await _repository.IsImageUsedByOtherProductsAsync(imageUrl, id);
                    if (!isImageShared)
                    {
                        bool fileExists = await _storageService.FileExistsAsync(containerName, blobName);
                        if (fileExists)
                        {
                            await _storageService.DeleteFileAsync(containerName, blobName);

                        }
                    }
                }
            }
        }
        _repository.Delete(product);
        return await _repository.SaveChangesAsync();
    }
    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
	{
		var product = await _repository.GetByIdAsync(id);
		if (product == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Product", id));

		var result = _mapper.Map<ProductDto>(product);
		return result;
	}


	public async Task<bool> CreateAsync(ProductDto model)
	{
		string containerName = ContainerName.product.ToString();
		List<string> pictureUrls = new List<string>();

		if (model.Images != null && model.Images.Any())
		{
			foreach (var file in model.Images!)
			{
				string fileName = file.FileName;
                string fileUrl;
                bool fileExists = await _storageService.FileExistsAsync(containerName, fileName);
                if (fileExists)
                {
                    fileUrl = await _storageService.GetFileAsync(containerName, fileName);
                }
                else
                {
                    fileUrl = await _storageService.UploadFileAsync(containerName, file);
                }
                pictureUrls.Add(fileUrl);
			}
		}

		model.PictureUrl = string.Join(",", pictureUrls);

		var product = _mapper.Map<Product>(model);
		product.PictureUrl = model.PictureUrl;

		_repository.Create(product);
		await _repository.SaveChangesAsync();
        product.CreatedAt = DateTime.Now;
        _repository.Update(product);
        return true;
	}

	public async Task<List<ProductDto>> GetProductsAsync()
	{
		var products = await _repository.GetAllAsync();
        var sortedProducts = products.OrderByDescending(p => p.CreatedAt).ToList();
        return _mapper.Map<List<ProductDto>>(products);
	}

	public async Task<List<ProductDto>> SearchProductsAsync(string keyword)
	{
		var products = await _repository.SearchProductsAsync(keyword);
		return _mapper.Map<List<ProductDto>>(products);
	}
	protected string? picture;

    public async Task<bool> UpdateAsync(ProductDto model)
    {
        string containerName = ContainerName.product.ToString();

        var existingProduct = await _repository.GetByIdAsync(model.Id);
        if (existingProduct == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Product", model.Id));

        List<string> newPictureUrls = new List<string>();

        if (model.Images is not null && model.Images.Count > 0)
        {
            var newImageNames = model.Images.Select(file => file.FileName).ToList();
            var oldImageNames = existingProduct.PictureUrl?
                .Split(',')
                .Select(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) ? Path.GetFileName(uri.LocalPath) : url)
                .ToList() ?? new List<string>();

            bool isSameImages = newImageNames.SequenceEqual(oldImageNames);

            if (isSameImages)
            {
                var updated = _mapper.Map(model, existingProduct);
                updated.PictureUrl = existingProduct.PictureUrl;
                _repository.Update(updated);
                await _repository.SaveChangesAsync();
                return true;
            }


            if (!string.IsNullOrEmpty(existingProduct.PictureUrl))
            {
                var oldImageUrls = existingProduct.PictureUrl.Split(',');

                foreach (var imageUrl in oldImageUrls)
                {
                    if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                    {
                        string oldBlobName = Path.GetFileName(uri.LocalPath);

                        bool fileExists = await _storageService.FileExistsAsync(containerName, oldBlobName);
                        if (fileExists)
                        {
                            bool isDeleted = await _storageService.DeleteFileAsync(containerName, oldBlobName);
                            if (!isDeleted)
                            {
                                throw new Exception($"Can't delete old picture: {oldBlobName}");
                            }
                        }
                    }
                }
            }

            newPictureUrls = await _storageService.SaveFilesAsync(containerName, model.Images);
        }
        else
        {
            newPictureUrls = existingProduct.PictureUrl?.Split(',').ToList() ?? new List<string>();
        }


        model.PictureUrl = string.Join(",", newPictureUrls);

        var updatedProduct = _mapper.Map(model, existingProduct);
        updatedProduct.PictureUrl = model.PictureUrl;
        _repository.Update(updatedProduct);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<List<string>> GetBrandAsync()
	{
		var brands = await _repository.GetBrandAsync();
		return brands;
	}

	public async Task<List<string>> GetTypeAsync()
	{
		var types = await _repository.GetTypeAsync();
		return types;
	}

    public async Task<List<ProductDto>> FilterProductsAsync(FilterProductInfo filterInfo)
    {
        var products = await _repository.FilterProductsAsync(filterInfo);
        return _mapper.Map<List<ProductDto>>(products);
    }
}
