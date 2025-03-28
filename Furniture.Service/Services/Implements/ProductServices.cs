
namespace Furniture.Service;
public class ProductServices(
    IProductRepository _repository,
    IFileStorageService _storageService,
    IMapper _mapper) : IProductServices
{
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Product", id));

            string containerName = ContainerName.product.ToString();

            if (!string.IsNullOrEmpty(product.PictureUrl))
            {
                var imageUrls = product.PictureUrl.Split(',');

                foreach (var imageUrl in imageUrls)
                {
                    try
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
                                    Console.WriteLine($"✅ picture deleted : {blobName}");
                                }
                                else
                                {
                                    Console.WriteLine($"⚠️ Picture does not exist: {blobName}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Do't have a valid URL: {imageUrl}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error deleting picture {imageUrl}: {ex.Message}");
                    }
                }
            }

            _repository.Delete(product);
            bool result = await _repository.SaveChangesAsync();
            Console.WriteLine($"✅ Product {id} deleted!");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting product {id}: {ex.Message}");
            throw;
        }
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
            foreach (var file in model.Images)
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
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductDto>> GetProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        var sortedProducts = products.OrderByDescending(p => p.CreatedAt).ToList();
        return _mapper.Map<List<ProductDto>>(sortedProducts);
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

		if (model.Images is not null && model.Images.Count > 0)
		{
			var newImageNames = model.Images.Select(file => file.FileName).ToList();
			var oldImageNames = existingProduct.PictureUrl.Split(',')
														  .Select(url => Path.GetFileName(new Uri(url).LocalPath))
														  .ToList();

			bool isSameImages = newImageNames.SequenceEqual(oldImageNames);

			if (isSameImages)
			{
				var updated = _mapper.Map(model, existingProduct);
				updated.PictureUrl = existingProduct.PictureUrl;
				_repository.Update(updated);
				await _repository.SaveChangesAsync();
				return true;
			}
		}


		if (!string.IsNullOrEmpty(existingProduct.PictureUrl))
		{
			var oldImageUrls = existingProduct.PictureUrl.Split(',');

			foreach (var imageUrl in oldImageUrls)
			{
				var uri = new Uri(imageUrl);
				string oldBlobName = Path.GetFileName(uri.LocalPath);

				bool fileExists = await _storageService.FileExistsAsync(containerName, oldBlobName);
				if (fileExists)
				{
					bool isDeleted = await _storageService.DeleteFileAsync(containerName, oldBlobName);
					if (!isDeleted)
					{
						throw new Exception($"can't delete old picture: {oldBlobName}");
					}
				}
			}
		}

		List<string> newPictureUrls = new List<string>();

		if (model.Images != null && model.Images.Count > 0)
		{
			newPictureUrls = await _storageService.SaveFilesAsync(containerName, model.Images);
		}

	
		model.PictureUrl = string.Join(",", newPictureUrls);

		var updatedProduct = _mapper.Map(model, existingProduct);
		updatedProduct.PictureUrl = model.PictureUrl ?? existingProduct.PictureUrl;
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
