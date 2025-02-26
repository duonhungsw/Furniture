using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;

namespace Furniture.Service.Services.Implements;

public class FileStorageService : IFileStorageService
{
	private readonly BlobServiceClient _blobServiceClient;

	public FileStorageService(string connectionString)
	{
		_blobServiceClient = new BlobServiceClient(connectionString);
	}

	public async Task<IEnumerable<string>> GetFileListAsync(string container)
	{
		var containerClient = _blobServiceClient.GetBlobContainerClient(container);
		if (!await containerClient.ExistsAsync())
		{
			return Enumerable.Empty<string>();
		}

		var blobs = new List<string>();
		await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
		{
			blobs.Add(blobItem.Name);
		}
		return blobs;
	}

	public async Task<string> GetFileAsync(string container, string blobName)
	{
		var containerClient = _blobServiceClient.GetBlobContainerClient(container);
		var blobClient = containerClient.GetBlobClient(blobName);

		if (!await blobClient.ExistsAsync())
		{
			throw new FileNotFoundException($"Blob '{blobName}' not found in container '{container}'.");
		}

		return blobClient.Uri.ToString();
	}

	public async Task<Stream> GetFileStreamAsync(string container, string blobName)
	{
		var containerClient = _blobServiceClient.GetBlobContainerClient(container);
		var blobClient = containerClient.GetBlobClient(blobName);

		if (!await blobClient.ExistsAsync())
		{
			throw new FileNotFoundException($"Blob '{blobName}' not found in container '{container}'.");
		}

		return await blobClient.OpenReadAsync();
	}

	public async Task<string> SaveFilesAsync(string container, List<IFormFile> files)
	{
		var containerClient = _blobServiceClient.GetBlobContainerClient(container);
		await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

		foreach (var file in files)
		{
			var contentType = GetContentType(file.FileName);
			var httpHeaders = new BlobHttpHeaders
			{
				ContentType = contentType
			};
			var blobClient = containerClient.GetBlobClient(file.FileName);
			using var stream = file.OpenReadStream();
			await blobClient.UploadAsync(stream, httpHeaders);
		}

		return $"Uploaded {files.Count} file(s) to container '{container}'.";
	}

	public async Task<string> UploadFileAsync(string containerName, IFormFile file)
	{
		var contentType = GetContentType(file.FileName);
		var httpHeaders = new BlobHttpHeaders
		{
			ContentType = contentType
		};
		var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
		await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

		var blobClient = containerClient.GetBlobClient(file.FileName);
		using var stream = file.OpenReadStream();
		await blobClient.UploadAsync(stream, httpHeaders);
		return blobClient.Uri.ToString();
	}

	public async Task<bool> DeleteFileAsync(string container, string blobName)
	{
		var containerClient = _blobServiceClient.GetBlobContainerClient(container);
		var blobClient = containerClient.GetBlobClient(blobName);

		return await blobClient.DeleteIfExistsAsync();
	}

	public async Task<bool> FileExistsAsync(string container, string blobName)
	{
		var containerClient = _blobServiceClient.GetBlobContainerClient(container);
		var blobClient = containerClient.GetBlobClient(blobName);

		return await blobClient.ExistsAsync();
	}

	public async Task<string> GenerateFileAccessUrlAsync(string container, string blobName, TimeSpan validDuration)
	{
		var containerClient = _blobServiceClient.GetBlobContainerClient(container);
		var blobClient = containerClient.GetBlobClient(blobName);

		if (!await blobClient.ExistsAsync())
		{
			throw new FileNotFoundException($"Blob '{blobName}' not found in container '{container}'.");
		}

		var sasBuilder = new BlobSasBuilder
		{
			BlobContainerName = container,
			BlobName = blobName,
			Resource = "b",
			ExpiresOn = DateTimeOffset.UtcNow.Add(validDuration)
		};

		sasBuilder.SetPermissions(BlobSasPermissions.Read);

		var uri = blobClient.GenerateSasUri(sasBuilder);
		return uri.ToString();
	}

	public async Task DeleteAllFilesAsync(string container)
	{
		var containerClient = _blobServiceClient.GetBlobContainerClient(container);

		if (!await containerClient.ExistsAsync())
		{
			throw new InvalidOperationException($"Container '{container}' does not exist.");
		}

		await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
		{
			var blobClient = containerClient.GetBlobClient(blobItem.Name);
			await blobClient.DeleteIfExistsAsync();
		}
	}
	private string GetContentType(string fileName)
	{
		var extension = Path.GetExtension(fileName).ToLower();
		return extension switch
		{
			".jpg" => "image/jpeg",
			".jpeg" => "image/jpeg",
			".png" => "image/png",
			".mp4" => "video/mp4",
			".mkv" => "video/x-matroska",
			_ => throw new NotSupportedException("Unsupported file extension")
		};
	}
}
