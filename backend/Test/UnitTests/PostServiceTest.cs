using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InteractHub.Application.DTOs.Post;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Services;
using InteractHub.Domain.Entities;
using Moq;
using Xunit;

namespace InteractHub.UnitTests.Services;

public class PostServiceTest
{
    private readonly Mock<IPostRepository> _mockRepo;
    private readonly PostService _service;

    public PostServiceTest()
    {
        _mockRepo = new Mock<IPostRepository>();
        _service = new PostService(_mockRepo.Object);
    }

    #region GetAllPosts Tests
    [Fact]
    public async Task GetAllPosts_ReturnsAllPosts()
    {
        var posts = new List<PostFeedItemDto>
        {
            new PostFeedItemDto { Id = Guid.NewGuid(), Content = "Post 1" },
            new PostFeedItemDto { Id = Guid.NewGuid(), Content = "Post 2" }
        };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(posts);

        var result = await _service.GetAllPosts();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockRepo.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetAllPosts_EmptyResult_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<PostFeedItemDto>());

        var result = await _service.GetAllPosts();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    #endregion

    #region GetPostById Tests
    [Fact]
    public async Task GetPostById_ValidId_ReturnsPost()
    {
        var postId = Guid.NewGuid();
        var expectedPost = new Post { Id = postId, Content = "Test Post", UserId = "user1" };
        _mockRepo.Setup(r => r.GetPostById(postId)).ReturnsAsync(expectedPost);

        var result = await _service.GetPostById(postId);

        Assert.NotNull(result);
        Assert.Equal(postId, result.Id);
        Assert.Equal("Test Post", result.Content);
    }

    [Fact]
    public async Task GetPostById_InvalidId_ReturnsNull()
    {
        var invalidId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetPostById(invalidId)).ReturnsAsync((Post?)null);

        var result = await _service.GetPostById(invalidId);

        Assert.Null(result);
    }
    #endregion

    #region CreatePost Tests
    [Fact]
    public async Task CreatePost_ValidData_ReturnsPost()
    {
        var userId = "user1";
        var createDto = new CreatePostDto 
        { 
            Content = "Hello World",
            Visibility = 0,
            Media = new List<MediaItemDto>(),
            Hashtags = new List<string>()
        };
        var expectedPost = new Post { Id = Guid.NewGuid(), Content = "Hello World", UserId = userId };
        _mockRepo.Setup(r => r.CreatePostWithDetailsAsync(It.IsAny<Post>(), It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(expectedPost);

        var result = await _service.CreatePost(userId, createDto);

        Assert.NotNull(result);
        Assert.Equal("Hello World", result.Content);
        Assert.Equal(userId, result.UserId);
        _mockRepo.Verify(r => r.CreatePostWithDetailsAsync(It.IsAny<Post>(), It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task CreatePost_WithMedia_CreatesWithMedia()
    {
        var userId = "user1";
        var createDto = new CreatePostDto 
        { 
            Content = "Post with media",
            Visibility = 0,
            Media = new List<MediaItemDto>
            {
                new MediaItemDto { MediaUrl = "https://example.com/image.jpg", MediaType = 0 }
            },
            Hashtags = new List<string>()
        };
        var expectedPost = new Post { Id = Guid.NewGuid(), Content = "Post with media", UserId = userId };
        _mockRepo.Setup(r => r.CreatePostWithDetailsAsync(It.IsAny<Post>(), It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(expectedPost);

        var result = await _service.CreatePost(userId, createDto);

        Assert.NotNull(result);
        _mockRepo.Verify(r => r.CreatePostWithDetailsAsync(
            It.IsAny<Post>(), 
            It.Is<List<PostMedia>>(m => m.Count == 1),
            It.IsAny<List<string>>()), 
            Times.Once);
    }

    [Fact]
    public async Task CreatePost_WithHashtags_CreatesWithHashtags()
    {
        var userId = "user1";
        var createDto = new CreatePostDto 
        { 
            Content = "Post with hashtags",
            Visibility = 0,
            Media = new List<MediaItemDto>(),
            Hashtags = new List<string> { "#test", "#demo" }
        };
        var expectedPost = new Post { Id = Guid.NewGuid(), Content = "Post with hashtags", UserId = userId };
        _mockRepo.Setup(r => r.CreatePostWithDetailsAsync(It.IsAny<Post>(), It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(expectedPost);

        var result = await _service.CreatePost(userId, createDto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreatePost_FailureFromRepo_ThrowsException()
    {
        var userId = "user1";
        var createDto = new CreatePostDto 
        { 
            Content = "Test",
            Visibility = 0,
            Media = null,
            Hashtags = null
        };
        _mockRepo.Setup(r => r.CreatePostWithDetailsAsync(It.IsAny<Post>(), It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()))
            .ReturnsAsync((Post?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreatePost(userId, createDto));
    }

    [Fact]
    public async Task CreatePost_NullHashtags_TreatsAsEmpty()
    {
        var userId = "user1";
        var createDto = new CreatePostDto 
        { 
            Content = "Test",
            Visibility = 0,
            Media = new List<MediaItemDto>(),
            Hashtags = null
        };
        var expectedPost = new Post { Id = Guid.NewGuid(), Content = "Test", UserId = userId };
        _mockRepo.Setup(r => r.CreatePostWithDetailsAsync(It.IsAny<Post>(), It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(expectedPost);

        var result = await _service.CreatePost(userId, createDto);

        Assert.NotNull(result);
    }
    #endregion

    #region UpdatePostAsync Tests
    [Fact]
    public async Task UpdatePostAsync_ValidData_ReturnsTrue()
    {
        var postId = Guid.NewGuid();
        var userId = "user1";
        var updateDto = new UpdatePostDto 
        { 
            Content = "Updated content",
            Visibility = 0,
            Media = new List<MediaItemDto>(),
            Hashtags = new List<string>()
        };
        _mockRepo.Setup(r => r.UpdatePostWithDetailsAsync(
            postId, userId, updateDto.Content, updateDto.Visibility, It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(true);

        var result = await _service.UpdatePostAsync(postId, userId, updateDto);

        Assert.True(result);
        _mockRepo.Verify(r => r.UpdatePostWithDetailsAsync(
            postId, userId, updateDto.Content, updateDto.Visibility, It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()), 
            Times.Once);
    }

    [Fact]
    public async Task UpdatePostAsync_Unauthorized_ReturnsFalse()
    {
        var postId = Guid.NewGuid();
        var wrongUserId = "hacker";
        var updateDto = new UpdatePostDto 
        { 
            Content = "Updated",
            Visibility = 0,
            Media = new List<MediaItemDto>(),
            Hashtags = new List<string>()
        };
        _mockRepo.Setup(r => r.UpdatePostWithDetailsAsync(
            postId, wrongUserId, updateDto.Content, updateDto.Visibility, It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(false);

        var result = await _service.UpdatePostAsync(postId, wrongUserId, updateDto);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdatePostAsync_WithMedia_Updates()
    {
        var postId = Guid.NewGuid();
        var userId = "user1";
        var updateDto = new UpdatePostDto 
        { 
            Content = "Updated",
            Visibility = 0,
            Media = new List<MediaItemDto>
            {
                new MediaItemDto { MediaUrl = "https://example.com/new.jpg", MediaType = 0 }
            },
            Hashtags = new List<string>()
        };
        _mockRepo.Setup(r => r.UpdatePostWithDetailsAsync(
            postId, userId, updateDto.Content, updateDto.Visibility, It.IsAny<List<PostMedia>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(true);

        var result = await _service.UpdatePostAsync(postId, userId, updateDto);

        Assert.True(result);
    }
    #endregion

    #region DeletePost Tests
    [Fact]
    public async Task DeletePost_ValidData_ReturnsTrue()
    {
        var postId = Guid.NewGuid();
        var userId = "user1";
        _mockRepo.Setup(r => r.DeletePost(postId, userId)).ReturnsAsync(true);

        var result = await _service.DeletePost(postId, userId);

        Assert.True(result);
        _mockRepo.Verify(r => r.DeletePost(postId, userId), Times.Once);
    }

    [Fact]
    public async Task DeletePost_InvalidId_ReturnsFalse()
    {
        var postId = Guid.NewGuid();
        var userId = "user1";
        _mockRepo.Setup(r => r.DeletePost(postId, userId)).ReturnsAsync(false);

        var result = await _service.DeletePost(postId, userId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeletePost_Unauthorized_ReturnsFalse()
    {
        var postId = Guid.NewGuid();
        var wrongUserId = "hacker";
        _mockRepo.Setup(r => r.DeletePost(postId, wrongUserId)).ReturnsAsync(false);

        var result = await _service.DeletePost(postId, wrongUserId);

        Assert.False(result);
    }
    #endregion

    #region SharePostAsync Tests

    [Fact]
    public async Task SharePostAsync_PostNotFound_ReturnsFalse()
    {
        var userId = "user1";
        var postId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetPostById(postId)).ReturnsAsync((Post?)null);

        var result = await _service.SharePostAsync(userId, postId, "Comment");

        Assert.False(result);
        _mockRepo.Verify(r => r.CreateShareAsync(It.IsAny<PostShare>()), Times.Never);
    }


    #region GetSharesByPostIdAsync Tests
    [Fact]
    public async Task GetSharesByPostIdAsync_ValidPostId_ReturnsShares()
    {
        var postId = Guid.NewGuid();
        var shares = new List<PostShare>
        {
            new PostShare { PostId = postId, SharerId = "user1" },
            new PostShare { PostId = postId, SharerId = "user2" }
        };
        _mockRepo.Setup(r => r.GetSharesByPostIdAsync(postId, 0)).ReturnsAsync(shares);

        var result = await _service.GetSharesByPostIdAsync(postId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetSharesByPostIdAsync_NoShares_ReturnsEmpty()
    {
        var postId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetSharesByPostIdAsync(postId, 0)).ReturnsAsync(new List<PostShare>());

        var result = await _service.GetSharesByPostIdAsync(postId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    #endregion

    #region GetSharedPostsByUserId Tests
    [Fact]
    public async Task GetSharedPostsByUserId_ValidUserId_ReturnsPosts()
    {
        var userId = "user1";
        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), Content = "Shared post 1" },
            new Post { Id = Guid.NewGuid(), Content = "Shared post 2" }
        };
        _mockRepo.Setup(r => r.GetSharedPostsByUserIdAsync(userId, 0)).ReturnsAsync(posts);

        var result = await _service.GetSharedPostsByUserId(userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetSharedPostsByUserId_NoShares_ReturnsEmpty()
    {
        var userId = "user1";
        _mockRepo.Setup(r => r.GetSharedPostsByUserIdAsync(userId, 0)).ReturnsAsync(new List<Post>());

        var result = await _service.GetSharedPostsByUserId(userId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    #endregion

    #region Get10Posts Tests
    [Fact]
    public async Task Get10Posts_ReturnsPagedPosts()
    {
        var posts = new List<PostFeedItemDto>
        {
            new PostFeedItemDto { Id = Guid.NewGuid(), Content = "Post 1" },
            new PostFeedItemDto { Id = Guid.NewGuid(), Content = "Post 2" }
        };
        _mockRepo.Setup(r => r.Get10Posts(0)).ReturnsAsync(posts);

        var result = await _service.Get10Posts();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Get10Posts_WithPage_ReturnsPaginatedPosts()
    {
        var posts = new List<PostFeedItemDto>();
        _mockRepo.Setup(r => r.Get10Posts(1)).ReturnsAsync(posts);

        var result = await _service.Get10Posts(1);

        Assert.NotNull(result);
        _mockRepo.Verify(r => r.Get10Posts(1), Times.Once);
    }
    #endregion

    #region Get10PostsByUserId Tests
    [Fact]
    public async Task Get10PostsByUserId_ValidUserId_ReturnsPosts()
    {
        var userId = "user1";
        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), UserId = userId, Content = "Post 1" },
            new Post { Id = Guid.NewGuid(), UserId = userId, Content = "Post 2" }
        };
        _mockRepo.Setup(r => r.Get10PostsByUserId(userId, 0)).ReturnsAsync(posts);

        var result = await _service.Get10PostsByUserId(userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Get10PostsByUserId_NoUser_ReturnsEmpty()
    {
        var userId = "nonexistent";
        _mockRepo.Setup(r => r.Get10PostsByUserId(userId, 0)).ReturnsAsync(new List<Post>());

        var result = await _service.Get10PostsByUserId(userId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    #endregion
#endregion
}