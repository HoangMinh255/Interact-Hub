using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InteractHub.Application.DTOs.Friendship;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Services;
using InteractHub.Domain.Entities;
using Moq;
using Xunit;

namespace InteractHub.UnitTests.Services;

public class FriendshipServiceTests
{
    private readonly Mock<IFriendshipRepository> _mockRepo;
    private readonly FriendshipService _service;

    public FriendshipServiceTests()
    {
        _mockRepo = new Mock<IFriendshipRepository>();
        _service = new FriendshipService(_mockRepo.Object);
    }

    [Fact]
    public async Task AcceptFriendRequest_ValidId_ReturnsTrue()
    {
        var requestId = Guid.NewGuid();
        _mockRepo.Setup(r => r.AcceptFriendRequest(requestId)).ReturnsAsync(true);

        var result = await _service.AcceptFriendRequest(requestId);

        Assert.True(result);
        _mockRepo.Verify(r => r.AcceptFriendRequest(requestId), Times.Once);
    }

    [Fact]
    public async Task AcceptFriendRequest_InvalidId_ReturnsFalse()
    {
        var invalidId = Guid.NewGuid();
        _mockRepo.Setup(r => r.AcceptFriendRequest(invalidId)).ReturnsAsync(false);

        var result = await _service.AcceptFriendRequest(invalidId);

        Assert.False(result);
    }

    [Fact]
    public async Task Get10FriendsByReceiverId_ValidUser_ReturnsList()
    {
        var userId = "user123";
        var expectedList = new List<FriendDto> { new FriendDto { FriendId = "user456" } };
        _mockRepo.Setup(r => r.Get10FriendsByReceiverId(userId, 0)).ReturnsAsync(expectedList);

        var result = await _service.Get10FriendsByReceiverId(userId, 0);

        Assert.Single(result);
    }

    [Fact]
    public async Task CreateFriendRequest_ValidData_ReturnsCreatedFriendship()
    {
        // Sửa lỗi CS1503: Sử dụng đúng kiểu CreateFriendRequestDto
        var dto = new CreateFriendRequestDto { RequesterId = "user1", ReceiverId = "user2" };
        var friendship = new Friendship { RequesterId = "user1", ReceiverId = "user2", Status = 0 };
        
        _mockRepo.Setup(r => r.CreateFriendRequest(It.IsAny<Friendship>())).ReturnsAsync(friendship);

        var result = await _service.CreateFriendRequest(dto);

        Assert.NotNull(result);
        Assert.Equal("user1", result.RequesterId);
        Assert.Equal(0, result.Status);
    }

    [Fact]
    public async Task Get10FriendsByReceiverId_PageOutOfRange_ReturnsEmptyList()
    {
        var userId = "user123";
        _mockRepo.Setup(r => r.Get10FriendsByReceiverId(userId, 999)).ReturnsAsync(new List<FriendDto>());

        var result = await _service.Get10FriendsByReceiverId(userId, 999);

        Assert.Empty(result);
    }
}