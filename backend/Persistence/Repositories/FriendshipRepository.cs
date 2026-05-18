using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Persistence.Data;
using InteractHub.Application.DTOs.Friendship;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace InteractHub.Persistence.Repositories;
public class FriendshipRepository : IFriendshipRepository
{
    private readonly AppDbContext _context;
    public FriendshipRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IList<Friendship>> GetAll()
    {
        return await _context.Friendships.ToListAsync();
    }
    public async Task<IList<FriendDto>> Get10FriendRequestByReceiverId(string ReceiverId, int page = 0)
    {
        return await _context.Friendships.Where(f => f.ReceiverId == ReceiverId && f.Status == 0 && f.IsBlocked == false)
                                         .Include(f => f.Requester)
                                         .Skip(page*10)
                                         .Take(10)
                                         .Select(f => new FriendDto
                                         {
                                            FriendshipId = f.Id,
                                            // Nếu tôi là người gửi -> Bạn tôi là Receiver. Ngược lại, bạn tôi là Requester.
                                            FriendId = f.RequesterId == ReceiverId ? f.ReceiverId : f.RequesterId,
                                            FriendName = f.RequesterId == ReceiverId ? f.Receiver.FullName : f.Requester.FullName,
                                            FriendAvatar = f.RequesterId == ReceiverId ? f.Receiver.AvatarUrl : f.Requester.AvatarUrl
                                         })
                                         .ToListAsync();
    }
    public async Task<IList<FriendDto>> Get10FriendsByReceiverId(string ReceiverId, int page = 0)   
    {
        return await _context.Friendships.Where(f => (f.ReceiverId == ReceiverId || f.RequesterId == ReceiverId) && f.Status == 1 && f.IsBlocked == false)
                                         .Include(f => f.Requester)
                                         .Skip(page*10)
                                         .Take(10)
                                         .Select(f => new FriendDto
                                         {
                                            FriendshipId = f.Id,
                                            // Nếu tôi là người gửi -> Bạn tôi là Receiver. Ngược lại, bạn tôi là Requester.
                                            FriendId = f.RequesterId == ReceiverId ? f.ReceiverId : f.RequesterId,
                                            FriendName = f.RequesterId == ReceiverId ? f.Receiver.FullName : f.Requester.FullName,
                                            FriendAvatar = f.RequesterId == ReceiverId ? f.Receiver.AvatarUrl : f.Requester.AvatarUrl
                                         })
                                         .ToListAsync();
    }
    public async Task<Friendship> CreateFriendRequest(Friendship friendship)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async() => {
            await _context.Friendships.AddAsync(friendship);
            await SaveChanges();
            return friendship;
        });
    }
    public async Task<bool> BlockFriend(string RequesterId, string ReceiverId)
    {
        var existingFriendRequest = await _context.Friendships.FirstOrDefaultAsync(f => f.RequesterId == RequesterId && f.ReceiverId == ReceiverId && f.IsDeleted == false );
        if(existingFriendRequest == null || existingFriendRequest.IsBlocked == true)
        {
            return false;
        }
        existingFriendRequest.UpdatedAt = DateTime.UtcNow;
        existingFriendRequest.IsBlocked = true;
        await SaveChanges();
        return true;
    }
    public async Task<bool> AcceptFriendRequest(Guid id)
    {
        var existingFriendRequest = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id && f.Status == 0);
        if(existingFriendRequest == null || existingFriendRequest.IsBlocked == true)
        {
            return false;
        }
        existingFriendRequest.UpdatedAt = DateTime.UtcNow;
        existingFriendRequest.Status = 1;
        await SaveChanges();
        return true;
    }

    public async Task<bool> RejectFriendRequest(Guid id)
    {
        var existingFriendRequest = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id && f.Status == 0);
        if(existingFriendRequest == null || existingFriendRequest.IsBlocked == true)
        {
            return false;
        }
        existingFriendRequest.UpdatedAt = DateTime.UtcNow;
        existingFriendRequest.IsDeleted = true;
        await SaveChanges();
        return true;
    }

    public async Task<bool> RemoveFriend(Guid id)
    {
        var existingFriendRequest = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id);
        if(existingFriendRequest == null || existingFriendRequest.IsBlocked == true)
        {
            return false;
        }
        _context.Friendships.Remove(existingFriendRequest);
        await SaveChanges();
        return true;
    }

    public async Task<IList<FriendDto>> Get10SuggestionsByUserId(string userId, int page = 0)
    {
        // Get all users who are:
        // - Not the current user
        // - Not already friends with the current user
        // - Do not have a pending friend request with the current user
        // - Are not blocked
        
        var friendIds = await _context.Friendships
            .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) && f.IsBlocked == false && f.IsDeleted == false)
            .Select(f => f.RequesterId == userId ? f.ReceiverId : f.RequesterId)
            .ToListAsync();

        return await _context.Users
            .Where(u => u.Id != userId && !friendIds.Contains(u.Id))
            .Skip(page * 10)
            .Take(10)
            .Select(u => new FriendDto
            {
                FriendshipId = Guid.Empty,
                FriendId = u.Id,
                FriendName = u.FullName,
                FriendAvatar = u.AvatarUrl
            })
            .ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}