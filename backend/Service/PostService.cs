using Microsoft.AspNetCore.Mvc;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;

    public PostService(IPostRepository postRepository, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public async Task<IList<Post>> GetAllPosts()
    {
        return await _postRepository.GetAll();
    }
    public async Task<Post?> GetPostById(int id)
    {
        return await _postRepository.GetPostById(id);
    }
    public async Task<IActionResult> CreatePost(Post post)
    {
        return await _postRepository.CreatePost(post);
    }
    public async Task<IActionResult> UpdatePost(Post post)
    {
        return await _postRepository.UpdatePost(post);
    }
    public async Task<IActionResult> DeletePost(Post post)
    {
        return await _postRepository.DeletePost(post);
    }
}