
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<AppUser>> GetAll()
    {
        return await _context.AppUsers.ToListAsync();
    }

    public async Task<AppUser?> GetUserById(string id)
    {
        return await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
    }
}