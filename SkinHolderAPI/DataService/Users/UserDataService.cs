using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.Users;

public interface IUserDataService
{
    Task<User?> GetByEmailAsync(string username);
}

public class UserDataService(SkinHolderDbContext context) : IUserDataService
{
    private readonly SkinHolderDbContext _context = context;

    public async Task<User?> GetByEmailAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}
