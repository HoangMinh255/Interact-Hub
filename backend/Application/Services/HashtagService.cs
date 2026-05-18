using InteractHub.Application.DTOs.Hashtag;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;

namespace InteractHub.Application.Services;

public class HashtagService : IHashtagService
{
    private readonly IHashtagRepository _hashtagRepository;
    public HashtagService(IHashtagRepository hashtagRepository)
    {
        _hashtagRepository = hashtagRepository;
    }
    public async Task<IList<Hashtag>> GetAll()
    {
        return await _hashtagRepository.GetAll();
    }
    public async Task<IList<TrendingHashtagDto>> Get5TrendingHashtags()
    {
        return await _hashtagRepository.Get5TrendingHashtags();
    }
}