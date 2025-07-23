using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.Registros;

public interface IRegistrosDataService
{
    Task<List<Registro>?> GetRegistrosAsync(int userId);
    Task<Registro?> GetRegistroAsync(long registroId);
    Task<Registro?> GetLastRegistroAsync(int userId);
}

public class RegistrosDataService(SkinHolderDbContext context) : IRegistrosDataService
{
    private readonly SkinHolderDbContext _context = context;

    public async Task<List<Registro>?> GetRegistrosAsync(int userId)
    {
        return await _context.Registros.Where(r => r.Userid == userId).ToListAsync();
    }

    public async Task<Registro?> GetRegistroAsync(long registroId)
    {
        return await _context.Registros.Include(r => r.Itemprecios).FirstOrDefaultAsync(r => r.Registroid == registroId);
    }

    public async Task<Registro?> GetLastRegistroAsync(int userId)
    {
        return await _context.Registros.Where(r => r.Userid == userId).OrderByDescending(r => r.Fechahora).FirstOrDefaultAsync();
    }
}
