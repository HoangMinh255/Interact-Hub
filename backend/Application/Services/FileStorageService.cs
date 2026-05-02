using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InteractHub.Application.Services;

public sealed class FileStorageService : IFileStorageService
{
    private readonly BlobStorageOptions _options;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(
        IOptions<BlobStorageOptions> options,
        ILogger<FileStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<(string BlobName, string Url)> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        if (content is null) throw new ArgumentNullException(nameof(content));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.", nameof(fileName));
        if (string.IsNullOrWhiteSpace(containerName)) containerName = _options.ContainerName;

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            throw new InvalidOperationException("Azure Blob connection string is missing.");

        var extension = Path.GetExtension(fileName);
        var blobName = $"{Guid.NewGuid():N}{extension}";
        var client = new BlobServiceClient(_options.ConnectionString);
        var container = client.GetBlobContainerClient(containerName);

        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var blobClient = container.GetBlobClient(blobName);

        var headers = new BlobHttpHeaders
        {
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType
        };

        _logger.LogInformation("Uploading blob {BlobName} to container {ContainerName}", blobName, containerName);

        await blobClient.UploadAsync(content, new BlobUploadOptions
        {
            HttpHeaders = headers
        }, cancellationToken);

        return (blobName, blobClient.Uri.ToString());
    }

    public async Task DeleteAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            return;

        if (string.IsNullOrWhiteSpace(containerName))
            containerName = _options.ContainerName;

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            throw new InvalidOperationException("Azure Blob connection string is missing.");

        var client = new BlobServiceClient(_options.ConnectionString);
        var container = client.GetBlobContainerClient(containerName);
        var blobClient = container.GetBlobClient(blobName);

        _logger.LogInformation("Deleting blob {BlobName} from container {ContainerName}", blobName, containerName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
