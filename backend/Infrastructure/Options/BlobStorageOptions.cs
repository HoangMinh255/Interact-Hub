namespace InteractHub.Infrastructure.Options;

public sealed class BlobStorageOptions
{
    public const string SectionName = "AzureBlob";

    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = "media";

    public string LocalStoragePath { get; set; } = "uploads";
}
