using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.Users;

public interface IUserDataService
{
    Task<User?> GetByEmailAsync(string username);
    Task<User?> GetByIdAsync(int userId);
    Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash);
    Task<bool> DeactivateUserAsync(int userId);
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

    public async Task<User?> GetByIdAsync(int userId)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetByIdAsync para userId={UserId}", userId);
            return null;
        }
    }

    public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
            if (user == null) return false;

            user.Passwordhash = newPasswordHash;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UpdatePasswordAsync para userId={UserId}", userId);
            return false;
        }
    }

    public async Task<bool> DeactivateUserAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
            if (user == null) return false;

            user.Isactive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en DeactivateUserAsync para userId={UserId}", userId);
            return false;
        }
    }
}
