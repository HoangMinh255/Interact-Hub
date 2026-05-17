using InteractHub.Application.Common;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Domain.Entities;
using InteractHub.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace InteractHub.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(
            x => x.Email == email || x.NormalizedEmail == email.ToUpper(),
            cancellationToken);
    }

    public Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(
            x => x.UserName == userName || x.NormalizedUserName == userName.ToUpper(),
            cancellationToken);
    }

    public async Task<PagedResult<ApplicationUser>> SearchAsync(
        string? keyword,
        int page,
        int pageSize,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<ApplicationUser> query = _context.Users.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var filter = keyword.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.FullName, $"%{filter}%") ||
                EF.Functions.Like(x.UserName ?? string.Empty, $"%{filter}%") ||
                EF.Functions.Like(x.Email ?? string.Empty, $"%{filter}%"));
        }

        var totalItems = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResult<ApplicationUser>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public void Update(ApplicationUser user)
    {
        _context.Users.Update(user);
    }

    public void Remove(ApplicationUser user)
    {
        _context.Users.Remove(user);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
