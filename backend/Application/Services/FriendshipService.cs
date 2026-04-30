using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InteractHub.Application.DTOs.Friendship;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.Interfaces.Repositories;

namespace InteractHub.Application.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepository;
    public FriendshipService(IFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }
    public async Task<IList<Friendship>> GetAll()
    {
        return await _friendshipRepository.GetAll();
    }
    public async Task<IList<FriendDto>> Get10FriendRequestByReceiverId(string ReceiverId, int page)
    {
        return await _friendshipRepository.Get10FriendRequestByReceiverId(ReceiverId,page);
    }
    public async Task<IList<FriendDto>> Get10FriendsByReceiverId(string ReceiverId, int page)
    {
        return await _friendshipRepository.Get10FriendsByReceiverId(ReceiverId, page);
    }
    public async Task<Friendship> CreateFriendRequest(CreateFriendRequestDto createFriendRequestDto)
    {
        var friendRequest = new Friendship
        {
            Id = Guid.NewGuid(),
            RequesterId = createFriendRequestDto.RequesterId,
            ReceiverId = createFriendRequestDto.ReceiverId,
            Status = (byte) 0,
            RequestedAt = createFriendRequestDto.RequestedAt,
            IsBlocked = false,
            CreatedAt = createFriendRequestDto.CreatedAt,
            IsDeleted = false
        };
        return await _friendshipRepository.CreateFriendRequest(friendRequest);
    }
    public async Task<bool> BlockFriend(string RequesterId, string ReceiverId)
    {
        return await _friendshipRepository.BlockFriend(RequesterId, ReceiverId);
    }
    public async Task<bool> AcceptFriendRequest(Guid id)
    {
        return await _friendshipRepository.AcceptFriendRequest(id);
    }
    public async Task<bool> RejectFriendRequest(Guid id)
    {
        return await _friendshipRepository.RejectFriendRequest(id);
    }

    public async Task<bool> RemoveFriend(Guid id)
    {
        return await _friendshipRepository.RemoveFriend(id);
    }
}