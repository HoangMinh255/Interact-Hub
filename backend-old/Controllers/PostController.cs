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

    [HttpGet("api/posts/{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            return NotFound();
        }

        var user = await _userRepository.GetUserById(post.UserId.ToString());

        var result = new
        {
            post.Id,
            post.Title,
            post.Content,
            post.ImageUrl,
            post.CreatedAt,
            User = user
        };

        return Ok(result);
    }

    [HttpPost("api/posts")]
    public async Task<IActionResult> CreatePost([FromBody] Post post)
    {
        return await _postRepository.CreatePost(post);
    }

    [HttpPut("api/posts/{id}")]
    public async Task<IActionResult> UpdatePost([FromBody] Post post)
    {
        return await _postRepository.UpdatePost(post);
    }

    [HttpDelete("api/posts/{id}")]
    public async Task<IActionResult> DeletePost([FromBody] Post post)
    {
        return await _postRepository.DeletePost(post);
    }
}