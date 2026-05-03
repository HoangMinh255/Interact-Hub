using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using InteractHub.Application.DTOs.Post;
namespace InteractHub.Application.Interfaces.Services;
public interface IPostService
{
    Task<IList<PostFeedItemDto>> GetAllPosts();
    Task<Post?> GetPostById(Guid id);
    Task<Post> CreatePost(string userId, CreatePostDto post);
    Task<bool> UpdatePostAsync(Guid postId, string userId, UpdatePostDto dto);
    Task<bool> DeletePost(Guid postId, string userId);
    Task<IList<PostFeedItemDto>> Get10Posts(int page = 0);
    Task<IList<Post>> Get10PostsByUserId(string userId, int page = 0);
    Task<bool> SharePostAsync(string userId, Guid postId, string? comment);
    Task<IList<PostShare>> GetSharesByPostIdAsync(Guid postId, int page = 0);
    Task<IList<Post>> GetSharedPostsByUserId(string userId, int page = 0);
}