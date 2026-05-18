
using InteractHub.Application.DTOs.Hashtag;
using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Repositories;
public interface IHashtagRepository
{
    Task<IList<Hashtag>> GetAll();
    Task<IList<TrendingHashtagDto>> Get5TrendingHashtags();
}