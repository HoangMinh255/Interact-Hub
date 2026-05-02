using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Persistence.Data;
using InteractHub.Application.DTOs.Friendship;
using InteractHub.Application.DTOs.User;
using InteractHub.Application.Common;
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
        return await _context.Friendships.Where(f => f.ReceiverId == ReceiverId && f.Status == 1 && f.IsBlocked == false)
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
    public async Task<PagedResult<UserSummaryDto>> Get10SuggestionsByUserId(string userId, int page = 0)
    {
        page = Math.Max(page, 0);

        var excludedUserIds = await _context.Friendships
            .Where(f => !f.IsDeleted && (f.RequesterId == userId || f.ReceiverId == userId))
            .Select(f => f.RequesterId == userId ? f.ReceiverId : f.RequesterId)
            .Distinct()
            .ToListAsync();

        excludedUserIds.Add(userId);

        var query = _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive && !excludedUserIds.Contains(u.Id))
            .OrderBy(u => u.FullName);

        var totalItems = await query.LongCountAsync();

        var items = await query
            .Skip(page * 10)
            .Take(10)
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                AvatarUrl = u.AvatarUrl,
                Bio = u.Bio,
                IsActive = u.IsActive
            })
            .ToListAsync();

        return new PagedResult<UserSummaryDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = 10,
            TotalItems = totalItems,
            TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / 10.0)
        };
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
        if(existingFriendRequest == null && existingFriendRequest.IsBlocked == true)
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
        var existingFriendRequest = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id && f.IsDeleted == false && f.Status == 0);
        if(existingFriendRequest == null && existingFriendRequest.IsBlocked == true)
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
        var existingFriendRequest = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id && f.IsDeleted == false && f.Status == 0);
        if(existingFriendRequest == null && existingFriendRequest.IsBlocked == true)
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
        var existingFriendRequest = await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id && f.IsDeleted == false && f.Status == 1);
        if(existingFriendRequest == null && existingFriendRequest.IsBlocked == true)
        {
            return false;
        }
        existingFriendRequest.UpdatedAt = DateTime.UtcNow;
        existingFriendRequest.Status = 0;
        existingFriendRequest.IsDeleted = true;
        await SaveChanges();
        return true;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}