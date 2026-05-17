
using InteractHub.Application.DTOs.Hashtag;
using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Services;
public interface IHashtagService
{
    Task<IList<Hashtag>> GetAll();
    Task<IList<TrendingHashtagDto>> Get5TrendingHashtags();
}