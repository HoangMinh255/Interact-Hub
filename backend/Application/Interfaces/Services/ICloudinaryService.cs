using Microsoft.AspNetCore.Http;

namespace InteractHub.Application.Interfaces.Services;
public interface ICloudinaryService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<bool> DeleteMediaAsync(string publicId);
}