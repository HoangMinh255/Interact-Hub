namespace InteractHub.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<(string BlobName, string Url)> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        string containerName,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default);
}
