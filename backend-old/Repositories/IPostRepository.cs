using Microsoft.AspNetCore.Mvc;

public interface IPostRepository
{
    Task<IList<Post>> GetAll();
    Task<Post?> GetPostById(int id);
    Task<IActionResult> CreatePost(Post post);
    Task<IActionResult> UpdatePost(Post post);
    Task<IActionResult> DeletePost(Post post);
    Task<IList<Post>> GetFirst10Post(int page = 0);
    Task<IList<Post>> GetPostsByUserId(int userId);
}