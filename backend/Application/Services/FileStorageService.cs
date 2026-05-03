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

        try
        {
            // Get base upload directory
            var uploadDir = Path.Combine(_options.LocalStoragePath ?? "uploads", containerName);
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Generate unique filename
            var extension = Path.GetExtension(fileName);
            var blobName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadDir, blobName);

            // Save file to disk
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await content.CopyToAsync(fileStream, cancellationToken);
            }

            // Return relative URL (assuming /uploads path is served by the API)
            var url = $"/uploads/{containerName}/{blobName}";

            _logger.LogInformation("Uploaded file {BlobName} to {FilePath}", blobName, filePath);

            return (blobName, url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
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

        try
        {
            var filePath = Path.Combine(_options.LocalStoragePath ?? "uploads", containerName, blobName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Deleted file {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {BlobName}", blobName);
        }

        await Task.CompletedTask;
    }
}
