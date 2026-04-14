using Microsoft.AspNetCore.Mvc;

public class PostController : ControllerBase
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;

    public PostController(IPostRepository postRepository, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    [HttpGet("api/posts")]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _postRepository.GetAll();
        var users = await _userRepository.GetAll();

        var result = posts.Select(post => new
        {
            post.Id,
            post.Title,
            post.Content,
            post.ImageUrl,
            post.CreatedAt,
            User = users.FirstOrDefault(u => u.Id == post.UserId.ToString())
        });

        return Ok(result);
    }
}