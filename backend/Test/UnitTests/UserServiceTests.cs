using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using InteractHub.Application.Common;
using InteractHub.Application.Common.Exceptions;
using InteractHub.Application.DTOs.User;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.Services;
using InteractHub.Domain.Entities;
using Moq;
using Xunit;

namespace InteractHub.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IFileStorageService> _mockFileStorage;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockFileStorage = new Mock<IFileStorageService>();
        _service = new UserService(_mockUserRepo.Object, _mockFileStorage.Object);
    }

    #region GetByIdAsync Tests
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsUser()
    {
        var userId = "user123";
        var user = new ApplicationUser { Id = userId, Email = "test@test.com", UserName = "testuser" };
        _mockUserRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        var invalidId = "invalid";
        _mockUserRepo.Setup(r => r.GetByIdAsync(invalidId, It.IsAny<CancellationToken>())).ReturnsAsync((ApplicationUser?)null);

        var result = await _service.GetByIdAsync(invalidId);

        Assert.Null(result);
    }
    #endregion

    #region GetByEmailAsync Tests
    [Fact]
    public async Task GetByEmailAsync_ValidEmail_ReturnsUser()
    {
        var email = "test@test.com";
        var user = new ApplicationUser { Email = email, UserName = "testuser" };
        _mockUserRepo.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.GetByEmailAsync(email);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_InvalidEmail_ReturnsNull()
    {
        var invalidEmail = "notfound@test.com";
        _mockUserRepo.Setup(r => r.GetByEmailAsync(invalidEmail, It.IsAny<CancellationToken>())).ReturnsAsync((ApplicationUser?)null);

        var result = await _service.GetByEmailAsync(invalidEmail);

        Assert.Null(result);
    }
    #endregion

    #region GetByUserNameAsync Tests
    [Fact]
    public async Task GetByUserNameAsync_ValidUserName_ReturnsUser()
    {
        var userName = "testuser";
        var user = new ApplicationUser { UserName = userName, Email = "test@test.com" };
        _mockUserRepo.Setup(r => r.GetByUserNameAsync(userName, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.GetByUserNameAsync(userName);

        Assert.NotNull(result);
        Assert.Equal(userName, result.UserName);
    }

    [Fact]
    public async Task GetByUserNameAsync_InvalidUserName_ReturnsNull()
    {
        var invalidUserName = "notfound";
        _mockUserRepo.Setup(r => r.GetByUserNameAsync(invalidUserName, It.IsAny<CancellationToken>())).ReturnsAsync((ApplicationUser?)null);

        var result = await _service.GetByUserNameAsync(invalidUserName);

        Assert.Null(result);
    }
    #endregion

    #region SearchAsync Tests
    [Fact]
    public async Task SearchAsync_NullQuery_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SearchAsync(null));
    }

    
    

    #endregion

    #region UpdateProfileAsync Tests
    [Fact]
    public async Task UpdateProfileAsync_NullDto_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateProfileAsync("user1", null));
    }

    [Fact]
    public async Task UpdateProfileAsync_UserNotFound_ThrowsNotFoundException()
    {
        var dto = new UpdateUserProfileDto { FullName = "John Doe" };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateProfileAsync("user1", dto));
    }

    [Fact]
    public async Task UpdateProfileAsync_EmptyFullName_ThrowsException()
    {
        var dto = new UpdateUserProfileDto { FullName = "   " };
        var user = new ApplicationUser { Id = "user1" };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateProfileAsync("user1", dto));
    }

    [Fact]
    public async Task UpdateProfileAsync_FutureDateOfBirth_ThrowsException()
    {
        var futureDate = DateTime.UtcNow.AddYears(1);
        var dto = new UpdateUserProfileDto { FullName = "John Doe", DateOfBirth = futureDate };
        var user = new ApplicationUser { Id = "user1" };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateProfileAsync("user1", dto));
    }

    [Fact]
    public async Task UpdateProfileAsync_ValidData_UpdatesAndSaves()
    {
        var dto = new UpdateUserProfileDto { FullName = "John Doe", Bio = "Developer" };
        var user = new ApplicationUser { Id = "user1", FullName = "Old Name" };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateProfileAsync("user1", dto);

        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("Developer", result.Bio);
        _mockUserRepo.Verify(r => r.Update(It.IsAny<ApplicationUser>()), Times.Once);
        _mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_WithValidDateOfBirth_Updates()
    {
        var pastDate = DateTime.UtcNow.AddYears(-30);
        var dto = new UpdateUserProfileDto { FullName = "John Doe", DateOfBirth = pastDate };
        var user = new ApplicationUser { Id = "user1" };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateProfileAsync("user1", dto);

        Assert.NotNull(result);
        _mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion

    #region UploadAvatarAsync Tests
    [Fact]
    public async Task UploadAvatarAsync_NullStream_ThrowsException()
    {
        var dto = new UpdateUserProfileDto { FullName = "Test" };
        
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UploadAvatarAsync("user1", null, "test.jpg", "image/jpeg")
        );
    }

    [Fact]
    public async Task UploadAvatarAsync_EmptyFileName_ThrowsException()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UploadAvatarAsync("user1", stream, "", "image/jpeg")
        );
    }

    [Fact]
    public async Task UploadAvatarAsync_InvalidContentType_ThrowsException()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UploadAvatarAsync("user1", stream, "test.txt", "text/plain")
        );
    }

    [Fact]
    public async Task UploadAvatarAsync_UserNotFound_ThrowsException()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UploadAvatarAsync("user1", stream, "test.jpg", "image/jpeg")
        );
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    [InlineData("image/webp")]
    [InlineData("image/gif")]
    public async Task UploadAvatarAsync_ValidContentTypes_Updates(string contentType)
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var user = new ApplicationUser { Id = "user1" };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockFileStorage.Setup(s => s.UploadAsync(stream, "test.jpg", contentType, "avatars", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("blob", "https://example.com/test.jpg"));
        _mockUserRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.UploadAvatarAsync("user1", stream, "test.jpg", contentType);

        Assert.NotNull(result);
        Assert.Equal("https://example.com/test.jpg", result.AvatarUrl);
        _mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion

    #region DeactivateAsync Tests
    [Fact]
    public async Task DeactivateAsync_UserNotFound_ThrowsException()
    {
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeactivateAsync("user1"));
    }

    [Fact]
    public async Task DeactivateAsync_ActiveUser_Deactivates()
    {
        var user = new ApplicationUser { Id = "user1", IsActive = true };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.DeactivateAsync("user1");

        Assert.True(result);
        Assert.False(user.IsActive);
        _mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_AlreadyInactive_ReturnsFalse()
    {
        var user = new ApplicationUser { Id = "user1", IsActive = false };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.DeactivateAsync("user1");

        Assert.False(result);
        _mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    #endregion

    #region ReactivateAsync Tests
    [Fact]
    public async Task ReactivateAsync_UserNotFound_ThrowsException()
    {
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.ReactivateAsync("user1"));
    }

    [Fact]
    public async Task ReactivateAsync_InactiveUser_Reactivates()
    {
        var user = new ApplicationUser { Id = "user1", IsActive = false };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.ReactivateAsync("user1");

        Assert.True(result);
        Assert.True(user.IsActive);
        _mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReactivateAsync_AlreadyActive_ReturnsFalse()
    {
        var user = new ApplicationUser { Id = "user1", IsActive = true };
        _mockUserRepo.Setup(r => r.GetByIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.ReactivateAsync("user1");

        Assert.False(result);
        _mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    #endregion
}
