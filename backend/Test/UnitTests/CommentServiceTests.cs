using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InteractHub.Application.DTOs.Comment;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Services;
using InteractHub.Domain.Entities;
using Moq;
using Xunit;

namespace InteractHub.UnitTests.Services;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _mockRepo;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _mockRepo = new Mock<ICommentRepository>();
        _service = new CommentService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllComments()
    {
        var comments = new List<Comment>
        {
            new Comment { Id = Guid.NewGuid(), Content = "Comment 1" },
            new Comment { Id = Guid.NewGuid(), Content = "Comment 2" }
        };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(comments);

        var result = await _service.GetAllComments();

        Assert.Equal(2, result.Count);
        _mockRepo.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetAll_EmptyRepository_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Comment>());

        var result = await _service.GetAllComments();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCommentById_ValidId_ReturnsComment()
    {
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, Content = "Test Comment" };
        _mockRepo.Setup(r => r.GetCommentById(commentId)).ReturnsAsync(comment);

        var result = await _service.GetCommentById(commentId);

        Assert.NotNull(result);
        Assert.Equal("Test Comment", result.Content);
    }

    [Fact]
    public async Task GetCommentById_InvalidId_ReturnsNull()
    {
        var invalidId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetCommentById(invalidId)).ReturnsAsync((Comment?)null);

        var result = await _service.GetCommentById(invalidId);

        Assert.Null(result);
    }


    [Fact]
    public async Task CreateComment_ValidData_ReturnsCreatedComment()
    {
        var dto = new CreateCommentDto { Content = "New Comment", PostId = Guid.NewGuid() };
        var comment = new Comment { Id = Guid.NewGuid(), Content = "New Comment" };
        _mockRepo.Setup(r => r.CreateComment(It.IsAny<Comment>())).ReturnsAsync(comment);

        var result = await _service.CreateComment(dto);

        Assert.NotNull(result);
        Assert.Equal("New Comment", result.Content);
    }

    [Fact]
    public async Task UpdateComment_ValidData_ReturnsUpdated()
    {
        var commentId = Guid.NewGuid();
        var dto = new UpdateCommentDto { Content = "Updated Comment" };
        var comment = new Comment { Id = commentId, Content = "Updated Comment" };
        _mockRepo.Setup(r => r.UpdateComment(It.IsAny<Guid>(), It.IsAny<UpdateCommentDto>())).ReturnsAsync(comment);

        var result = await _service.UpdateComment(commentId, dto);

        Assert.NotNull(result);
        Assert.Equal("Updated Comment", result.Content);
    }

    [Fact]
    public async Task DeleteComment_ValidId_ReturnsTrue()
    {
        var commentId = Guid.NewGuid();
        _mockRepo.Setup(r => r.DeleteComment(commentId)).ReturnsAsync(true);

        var result = await _service.DeleteComment(commentId);

        Assert.True(result);
        _mockRepo.Verify(r => r.DeleteComment(commentId), Times.Once);
    }

    [Fact]
    public async Task DeleteComment_InvalidId_ReturnsFalse()
    {
        var invalidId = Guid.NewGuid();
        _mockRepo.Setup(r => r.DeleteComment(invalidId)).ReturnsAsync(false);

        var result = await _service.DeleteComment(invalidId);

        Assert.False(result);
    }

    [Fact]
    public async Task GetCommentsByPostId_ValidPostId_ReturnsAllComments()
    {
        var postId = Guid.NewGuid();
        var comments = new List<Comment>
        {
            new Comment { Id = Guid.NewGuid(), Content = "Comment 1" }
        };
        _mockRepo.Setup(r => r.GetCommentsByPostId(postId)).ReturnsAsync(comments);

        var result = await _service.GetCommentsByPostId(postId);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetCommentsByPostId_NoComments_ReturnsEmpty()
    {
        var postId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetCommentsByPostId(postId)).ReturnsAsync(new List<Comment>());

        var result = await _service.GetCommentsByPostId(postId);

        Assert.Empty(result);
    }
}
