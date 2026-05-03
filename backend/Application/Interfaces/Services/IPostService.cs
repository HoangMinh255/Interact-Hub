using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using InteractHub.Application.DTOs.Post;
namespace InteractHub.Application.Interfaces.Services;
public interface IPostService
{
    Task<IList<Post>> GetAllPosts();
    Task<Post?> GetPostById(Guid id);
    Task<Post> CreatePost(string userId, CreatePostDto post);
    Task<bool> UpdatePostAsync(Guid postId, string userId, UpdatePostDto dto);
    Task<bool> DeletePost(Guid postId, string userId);
    Task<IList<Post>> Get10Posts(int page = 0);
    Task<IList<Post>> Get10PostsByUserId(string userId, int page = 0);
}