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
    public async Task CreatePost(Post post)
    {
        _context.Posts.Add(post);
        await SaveChanges();
    }
    public async Task UpdatePost(Post post)
    {
        _context.Posts.Update(post);
        await SaveChanges();
    }
    public async Task DeletePost(Post post)
    {
        if (post != null)
        {
            post.IsDeleted = 1;
            _context.Posts.Update(post);           
        }
        await SaveChanges();
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