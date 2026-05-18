using InteractHub.Application.DTOs.Hashtag;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Domain.Entities;
using InteractHub.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace InteractHub.Persistence.Repositories;

public class HashtagRepository : IHashtagRepository
{
    private readonly AppDbContext _context;
    public HashtagRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IList<Hashtag>> GetAll()
    {
        return await _context.Hashtags.ToListAsync();
    }
    public async Task<IList<TrendingHashtagDto>> Get5TrendingHashtags()
    {
        return await _context.PostHashtags.Where(ph => ph.Post.IsDeleted == false)
                                          .GroupBy(ph => ph.Hashtag.Name)
                                          .OrderByDescending(g => g.Count())
                                          .Take(5)
                                          .Select(g => new TrendingHashtagDto
                                          {
                                              Tag = g.Key,
                                              Count = g.Count()
                                          })
                                          .ToListAsync();
    }

}