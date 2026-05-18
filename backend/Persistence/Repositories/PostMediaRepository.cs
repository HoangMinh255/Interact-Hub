using InteractHub.Domain.Entities;
using InteractHub.Persistence.Data;
using InteractHub.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
namespace InteractHub.Persistence.Repositories;

public class PostMediaRepository : IPostMediaRepository
{
    private readonly AppDbContext _context;

    public PostMediaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddPostMedia(PostMedia media)
    {
        await _context.PostMedia.AddAsync(media);
        await SaveChanges();
    }

    public async Task<IActionResult> DeletePostMedia(PostMedia media)
    {
        _context.PostMedia.Remove(media);
        await SaveChanges();
        return new OkResult();
    }

    public async Task<IList<PostMedia>> GetPostMediasByPostId(Guid postId)
    {
        return await _context.PostMedia.Where(m => m.PostId == postId).ToListAsync();
    }
    public async Task<IActionResult> UpdatePostMedia(PostMedia media)
    {
        _context.PostMedia.Update(media);
        await SaveChanges();
        return new OkResult();
    }
    public async Task<PostMedia?> GetPostMediaById(Guid id)
    {
        return await _context.PostMedia.FirstOrDefaultAsync(m => m.Id == id);
    }
    public async Task DeletePostMediaByPostId(Guid postId)
    {
        var medias = await _context.PostMedia.Where(m => m.PostId == postId).ToListAsync();
        _context.PostMedia.RemoveRange(medias);
        await SaveChanges();
    }

    public async Task<IActionResult> CreatePostMedia(PostMedia postMedia)
    {
        await _context.PostMedia.AddAsync(postMedia);
        await SaveChanges();
        return new OkResult();
    }


    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}