
namespace Furniture.Service;
public class ProductServices(
	IProductRepository _repository,
	IFileStorageService _storageService,
	IMapper _mapper) : IProductServices
{
	public async Task<bool> DeleteAsync(Guid id)
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
				var uri = new Uri(imageUrl);
				string blobName = Path.GetFileName(uri.LocalPath);

				bool fileExists = await _storageService.FileExistsAsync(containerName, blobName);
				if (fileExists)
				{
					bool isDeleted = await _storageService.DeleteFileAsync(containerName, blobName);
					if (!isDeleted)
					{
						throw new BadRequestException(string.Format(ErrorMessageBase.BadRequest, "Invalid data format"));
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
            foreach (var file in model.Images)
            {
                string fileName = file.FileName;
                bool fileExists = await _storageService.FileExistsAsync(containerName, fileName);
                if (fileExists)
                {
                    throw new Exception($"File '{fileName}' đã tồn tại trong hệ thống.");
                }
                string fileUrl = await _storageService.UploadFileAsync(containerName, file);
                pictureUrls.Add(fileUrl);
            }
        }

        model.PictureUrl = string.Join(",", pictureUrls);
        var product = _mapper.Map<Product>(model);
        product.PictureUrl = model.PictureUrl;

        _repository.Create(product);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductDto>> GetProductsAsync()
	{
		var products = await _repository.GetAllAsync();
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

        if (model.Images != null && model.Images.Count > 0)
        {
            var newImageNames = model.Images.Select(file => file.FileName).ToList();
            var oldImageNames = existingProduct.PictureUrl?
                .Split(',')
                .Select(url => Path.GetFileName(new Uri(url).LocalPath))
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

            // Nếu có ảnh mới, xóa ảnh cũ trước
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
                            throw new Exception($"Không thể xóa ảnh cũ: {oldBlobName}");
                        }
                    }
                }
            }

            // Lưu ảnh mới
            newPictureUrls = await _storageService.SaveFilesAsync(containerName, model.Images);
        }
        else
        {
            // Nếu không cập nhật ảnh mới, giữ nguyên ảnh cũ
            newPictureUrls = existingProduct.PictureUrl?.Split(',').ToList() ?? new List<string>();
        }

        // Cập nhật danh sách ảnh mới vào `PictureUrl`
        model.PictureUrl = string.Join(",", newPictureUrls);

        var updatedProduct = _mapper.Map(model, existingProduct);
        updatedProduct.PictureUrl = model.PictureUrl;
        _repository.Update(updatedProduct);
        await _repository.SaveChangesAsync();

        return true;
    }

}
