using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InteractHub.Application.Common;
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
    private readonly IOptions<IdentitySeedOptions> _seedOptions;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        var options = new Mock<IOptions<IdentitySeedOptions>>();
        options.Setup(o => o.Value).Returns(new IdentitySeedOptions());
        _seedOptions = options.Object;

        _authService = new AuthService(
            _mockUserManager.Object, 
            _mockJwtTokenService.Object, 
            _seedOptions);
    }

    #region RegisterAsync Tests
    [Fact]
    public async Task RegisterAsync_ValidData_ReturnsSuccess()
    {
        var registerDto = new RegisterRequestDto 
        { 
            Email = "new@test.com", 
            UserName = "newuser",
            FullName = "New User",
            Password = "Password123!" 
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(registerDto.UserName))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync(registerDto);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(registerDto.Email, result.Data.Email);
        Assert.Equal(registerDto.UserName, result.Data.UserName);
        Assert.Single(result.Data.Roles);
    }

    [Fact]
    public async Task RegisterAsync_CreateAsyncFails_ReturnsFail()
    {
        var registerDto = new RegisterRequestDto 
        { 
            Email = "new@test.com", 
            UserName = "newuser",
            FullName = "New User",
            Password = "Password123!" 
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(registerDto.UserName))
            .ReturnsAsync((ApplicationUser?)null);
        
        var errors = new[] { new IdentityError { Description = "Password too weak" } };
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var result = await _authService.RegisterAsync(registerDto);

        Assert.False(result.Success);
        Assert.Contains("Password too weak", result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_MultipleErrors_ReturnsAllErrors()
    {
        var registerDto = new RegisterRequestDto 
        { 
            Email = "new@test.com", 
            UserName = "newuser",
            FullName = "New User",
            Password = "Password123!" 
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(registerDto.UserName))
            .ReturnsAsync((ApplicationUser?)null);
        
        var errors = new[] 
        { 
            new IdentityError { Description = "Error 1" },
            new IdentityError { Description = "Error 2" }
        };
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var result = await _authService.RegisterAsync(registerDto);

        Assert.False(result.Success);
        Assert.Equal(2, result.Errors.Count);
    }
    #endregion

    #region LoginAsync Tests
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
    {
        var loginDto = new LoginRequestDto { Email = "test@test.com", Password = "Password123!" };
        var user = new ApplicationUser 
        { 
            Id = "user1",
            Email = "test@test.com", 
            UserName = "testuser",
            FullName = "Test User",
            IsActive = true
        };
        var roles = new List<string> { "User" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
        _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user, roles)).Returns("mock_token");

        var result = await _authService.LoginAsync(loginDto);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("mock_token", result.Data.AccessToken);
        Assert.Equal("test@test.com", result.Data.User.Email);
    }

    [Fact]
    public async Task LoginAsync_ValidatesEmailFormatFormatValidation()
    {
        var loginDto = new LoginRequestDto { Email = "test@test.com", Password = "Password123!" };
        var user = new ApplicationUser { Email = "test@test.com", UserName = "testuser", IsActive = true };
        var roles = new List<string> { "User" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
        _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user, roles)).Returns("token");

        var result = await _authService.LoginAsync(loginDto);

        Assert.True(result.Success);
        _mockUserManager.Verify(x => x.FindByEmailAsync(loginDto.Email), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ExpiryTimeIsSet()
    {
        var loginDto = new LoginRequestDto { Email = "test@test.com", Password = "Password123!" };
        var user = new ApplicationUser { Email = "test@test.com", UserName = "testuser", IsActive = true };
        var roles = new List<string> { "User" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
        _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user, roles)).Returns("token");

        var result = await _authService.LoginAsync(loginDto);

        Assert.True(result.Success);
        Assert.True(result.Data.ExpiresAtUtc > System.DateTime.UtcNow);
    }
    #endregion
}