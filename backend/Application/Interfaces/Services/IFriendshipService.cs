using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using InteractHub.Application.DTOs.Friendship;
using InteractHub.Application.DTOs.User;
using InteractHub.Application.Common;
namespace InteractHub.Application.Interfaces.Services;

public interface IFriendshipService
{
    Task<IList<Friendship>> GetAll();
    Task<IList<FriendDto>> Get10FriendRequestByReceiverId(string ReceiverId, int page);
    Task<IList<FriendDto>> Get10FriendsByReceiverId(string ReceiverId, int page);
    Task<PagedResult<UserSummaryDto>> Get10SuggestionsByUserId(string userId, int page);
    Task<Friendship> CreateFriendRequest(CreateFriendRequestDto createFriendRequestDto);
    Task<bool> BlockFriend(string RequesterId, string ReceiverId);
    Task<bool> AcceptFriendRequest(Guid id);
    Task<bool> RejectFriendRequest(Guid id);
    Task<bool> RemoveFriend(Guid id);
}