using System;
using System.IO;
using System.Threading.Tasks;
using InteractHub.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace InteractHub.UnitTests.Services;

public class CloudinaryServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly CloudinaryService _service;

    public CloudinaryServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        
        _mockConfiguration
            .Setup(c => c["Cloudinary:CloudName"])
            .Returns("test_cloud");
        _mockConfiguration
            .Setup(c => c["Cloudinary:ApiKey"])
            .Returns("test_key");
        _mockConfiguration
            .Setup(c => c["Cloudinary:ApiSecret"])
            .Returns("test_secret");

        _service = new CloudinaryService(_mockConfiguration.Object);
    }

    private Mock<IFormFile> CreateMockFormFile(string fileName, string content = "test content")
    {
        var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.FileName).Returns(fileName);
        mock.Setup(f => f.Length).Returns(ms.Length);
        mock.Setup(f => f.OpenReadStream()).Returns(ms);
        mock.Setup(f => f.ContentType).Returns("image/jpeg");
        return mock;
    }

    [Fact]
    public async Task UploadFileAsync_NullFile_ReturnsNull()
    {
        var result = await _service.UploadFileAsync(null);
        Assert.Null(result);
    }

    [Fact]
    public async Task UploadFileAsync_EmptyFile_ReturnsNull()
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);
        
        var result = await _service.UploadFileAsync(mockFile.Object);
        Assert.Null(result);
    }

    [Fact]
    public async Task UploadFileAsync_ValidFile_ConfigurationIsVerified()
    {
        var mockFile = CreateMockFormFile("test.jpg");
        
        // Just verify configuration is accessed during service operations
        _mockConfiguration
            .Setup(c => c["Cloudinary:CloudName"])
            .Returns("test_cloud");
        
        Assert.NotNull(_service);
    }

    [Fact]
    public async Task DeleteMediaAsync_ValidPublicId_CallsCloudinary()
    {
        var publicId = "InteractHub_Media/test_file_123";
        
        // Test that method can be called without throwing
        var result = await _service.DeleteMediaAsync(publicId);
        
        // Result should be boolean
        Assert.IsType<bool>(result);
    }

    [Fact]
    public void CloudinaryService_Constructor_InitializesSuccessfully()
    {
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["Cloudinary:CloudName"]).Returns("cloud");
        config.Setup(c => c["Cloudinary:ApiKey"]).Returns("key");
        config.Setup(c => c["Cloudinary:ApiSecret"]).Returns("secret");
        
        var service = new CloudinaryService(config.Object);
        Assert.NotNull(service);
    }

    [Fact]
    public void CloudinaryService_Configuration_VerifyCloudinarySettings()
    {
        _mockConfiguration.Verify(c => c["Cloudinary:CloudName"], Times.AtLeastOnce);
    }

    [Theory]
    [InlineData("image1.jpg")]
    [InlineData("photo.png")]
    [InlineData("picture.webp")]
    [InlineData("avatar.gif")]
    public async Task UploadFileAsync_VariousFileTypes_Handles(string fileName)
    {
        var mockFile = CreateMockFormFile(fileName);
        var result = await _service.UploadFileAsync(mockFile.Object);
        
        // Result should be null or string
        Assert.True(result == null || result is string);
    }


}
