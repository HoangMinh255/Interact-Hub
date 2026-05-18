using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using InteractHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InteractHub.Application.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        // Khởi tạo kết nối với Cloudinary bằng thông tin trong appsettings
        var acc = new Account(
            configuration["Cloudinary:CloudName"],
            configuration["Cloudinary:ApiKey"],
            configuration["Cloudinary:ApiSecret"]
        );

        _cloudinary = new Cloudinary(acc);
        _cloudinary.Api.Secure = true; // Đảm bảo trả về link HTTPS
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return null;

        var uploadResult = new ImageUploadResult();

        using (var stream = file.OpenReadStream())
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "InteractHub_Media",
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                Console.WriteLine($"---> CLOUDINARY TỪ CHỐI UPLOAD: {uploadResult.Error.Message}");
                return null;
            }
        }

        // Trả về link ảnh an toàn (HTTPS)
        return uploadResult.SecureUrl?.ToString();
    }

    public async Task<bool> DeleteMediaAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }
}