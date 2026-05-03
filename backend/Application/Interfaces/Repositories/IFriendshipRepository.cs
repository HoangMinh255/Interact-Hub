using InteractHub.Domain.Entities;
using InteractHub.Application.DTOs.Friendship;
using Microsoft.AspNetCore.Mvc;

namespace InteractHub.Application.Interfaces.Repositories;
public interface IFriendshipRepository
{
    Task<IList<Friendship>> GetAll();
    Task<IList<FriendDto>> Get10FriendRequestByReceiverId(string ReceiverId, int page);
    Task<IList<FriendDto>> Get10FriendsByReceiverId(string ReceiverId, int page);
    Task<Friendship> CreateFriendRequest(Friendship friendship);
    Task<bool> BlockFriend(string RequesterId, string ReceiverId);
    Task<bool> AcceptFriendRequest(Guid id);
    Task<bool> RejectFriendRequest(Guid id);
    Task<bool> RemoveFriend(Guid id);
    Task<IList<FriendDto>> Get10SuggestionsByUserId(string userId, int page);
    Task SaveChanges();
}