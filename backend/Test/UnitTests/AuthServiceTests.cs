using System.Collections.Generic;
using System.Threading.Tasks;
using InteractHub.Application.DTOs.Auth;
using InteractHub.Application.Interfaces.Infrastructure;
using InteractHub.Application.Services;
using InteractHub.Domain.Entities;
using InteractHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace InteractHub.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // 1. Mock UserManager (Dùng null! để xóa cảnh báo CS8625)
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        
        // 2. Mock JwtTokenService
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        // 3. Mock IOptions<IdentitySeedOptions> (Thêm tham số thứ 3 bị thiếu)
        var options = new Mock<IOptions<IdentitySeedOptions>>();
        options.Setup(o => o.Value).Returns(new IdentitySeedOptions());

        // Tiêm đúng 3 dependency mà AuthService cần
        _authService = new AuthService(
            _mockUserManager.Object, 
            _mockJwtTokenService.Object, 
            options.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
    {
        var loginDto = new LoginRequestDto { Email = "test@test.com", Password = "Password123!" };
        var user = new ApplicationUser { Email = "test@test.com", UserName = "testuser" };
        var roles = new List<string> { "User" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        
        // Vì AuthService không dùng SignInManager, nó sẽ dùng CheckPasswordAsync của UserManager
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
                          
        _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
        _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user, It.IsAny<IEnumerable<string>>())).Returns("mock_token");

        var result = await _authService.LoginAsync(loginDto);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        // Đổi .Token thành .AccessToken để khớp với DTO của bạn
        Assert.Equal("mock_token", result.Data.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        var loginDto = new LoginRequestDto { Email = "test@test.com", Password = "WrongPassword!" };
        var user = new ApplicationUser { Email = "test@test.com", UserName = "testuser" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(false);

        var result = await _authService.LoginAsync(loginDto);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task LoginAsync_EmailNotFound_ReturnsNull()
    {
        var loginDto = new LoginRequestDto { Email = "notfound@test.com", Password = "Password123!" };
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email)).ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.LoginAsync(loginDto);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task RegisterAsync_ValidData_ReturnsTrue()
    {
        var registerDto = new RegisterRequestDto { Email = "new@test.com", Password = "Password123!" };
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
                        .ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync(registerDto);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsFalse()
    {
        var registerDto = new RegisterRequestDto { Email = "exist@test.com", Password = "Password123!" };
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email already exists" }));

        var result = await _authService.RegisterAsync(registerDto);

        Assert.False(result.Success);
    }
}