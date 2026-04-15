using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.Users;

public interface IUserDataService
{
    Task<User?> GetByEmailAsync(string username);
}

public class UserDataService(SkinHolderDbContext context, ILogger<UserDataService> logger) : IUserDataService
{
    private readonly SkinHolderDbContext _context = context;
    private readonly ILogger<UserDataService> _logger = logger;

    public async Task<User?> GetByEmailAsync(string username)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByEmailAsync para username={Username}", username);
            return null;
        }
    }
}
