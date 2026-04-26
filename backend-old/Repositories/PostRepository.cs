using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;
    public PostRepository(AppDbContext context)
    {
        _context=context;
    }
    public async Task<IList<Post>> GetAll()
    {
        return await _context.Posts.ToListAsync();
    }
    public async Task<Post?> GetPostById(int id)
    {
        return await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == 0);
    }
    public async Task<IActionResult> CreatePost(Post post)
    {
        _context.Posts.Add(post);
        await SaveChanges();
        return new OkResult();
    }
    public async Task<IActionResult> UpdatePost(Post post)
    {
        var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == post.Id && p.IsDeleted == 0);
        if (existingPost == null)
        {
            return new NotFoundResult();
        }
        _context.Posts.Update(existingPost);
        await SaveChanges();
        return new OkResult();

    }
    public async Task<IActionResult> DeletePost(Post post)
    {
        var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == post.Id && p.IsDeleted == 0);
        if (existingPost == null)
        {
            return new NotFoundResult();
        }
        existingPost.IsDeleted = 1;
        _context.Posts.Update(existingPost);
        await SaveChanges();
        return new OkResult();        
    }
    public async Task<IList<Post>> GetFirst10Post(int page = 0)
    {
        return await _context.Posts.Where(p => p.IsDeleted == 0 ).Skip(page*10).Take(10).ToListAsync();
    }
    public async Task<IList<Post>> GetPostsByUserId(int userId)
    {
        return await _context.Posts.Where(p => p.UserId == userId && p.IsDeleted == 0).ToListAsync();
    }
    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

}