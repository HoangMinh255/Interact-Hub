using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.DTOs.Post;
namespace InteractHub.Application.Interfaces.Repositories;

public interface IPostRepository
{
    Task<IList<PostFeedItemDto>> GetAll();
    Task<Post?> GetPostById(Guid id);
    Task<Post> CreatePost(Post post);
    Task<bool> UpdatePostWithDetailsAsync(Guid postId, string userId, string content, int visibility, List<PostMedia>? newMedias, List<string>? newHashtags);
    Task<bool> DeletePost(Guid postId, string userId);
    Task<IList<PostFeedItemDto>> Get10Posts(int page = 0);
    Task<IList<Post>> Get10PostsByUserId(string userId, int page = 0);
    Task<PostShare> CreateShareAsync(PostShare share);
    Task<IList<PostShare>> GetSharesByPostIdAsync(Guid postId, int page = 0);
    Task<IList<Post>> GetSharedPostsByUserIdAsync(string userId, int page = 0);
    Task<Post> CreatePostWithDetailsAsync(Post post, List<PostMedia> medias, List<string> hashtags);
}