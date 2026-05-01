using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
namespace InteractHub.Application.Interfaces.Repositories;
public interface IPostMediaRepository
{
    Task<IActionResult> CreatePostMedia(PostMedia postMedia);
    Task<IActionResult> UpdatePostMedia(PostMedia postMedia);
    Task<IActionResult> DeletePostMedia(PostMedia postMedia);
    Task<IList<PostMedia>> GetPostMediasByPostId(Guid postId);
}