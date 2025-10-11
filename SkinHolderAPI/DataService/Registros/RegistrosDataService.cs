using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.Application.Log;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;
using SkinHolderAPI.Utils;

namespace SkinHolderAPI.DataService.Registros;

public interface IRegistrosDataService
{
    Task<List<Registro>?> GetRegistrosAsync(int userId);
    Task<Registro?> GetRegistroAsync(long registroId);
    Task<Registro?> GetLastRegistroAsync(int userId);
    Task<long> CreateRegistroAsync(Registro registro);
    Task<bool> DeleteRegistroAsync(Registro registro);
}

public class RegistrosDataService(SkinHolderDbContext context, ILogLogic logLogic) : IRegistrosDataService
{
    private readonly SkinHolderDbContext _context = context;
    private readonly ILogLogic _logLogic = logLogic;

    private async Task<T?> SafeExecuteAsync<T>(Func<Task<T>> action, string methodName, int userId = 0)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            await _logLogic.AddLogAsync(
                LogBuilder.BuildLoggerDto(
                    $"Exception in {methodName}: {ex.Message}",
                    LogType.Error,
                    LogPlace.Api,
                    userId
                )
            );
            return default;
        }
    }

    public async Task<List<Registro>?> GetRegistrosAsync(int userId)
    {
        return await SafeExecuteAsync(
            async () => await _context.Registros.Where(r => r.Userid == userId).ToListAsync(),
            nameof(GetRegistrosAsync),
            userId
        );
    }

    public async Task<Registro?> GetRegistroAsync(long registroId)
    {
        return await SafeExecuteAsync(
            async () => await _context.Registros.Include(r => r.Itemprecios)
                                    .FirstOrDefaultAsync(r => r.Registroid == registroId),
            nameof(GetRegistroAsync)
        );
    }

    public async Task<Registro?> GetLastRegistroAsync(int userId)
    {
        return await SafeExecuteAsync(
            async () => await _context.Registros
                          .Where(r => r.Userid == userId)
                          .OrderByDescending(r => r.Fechahora)
                          .FirstOrDefaultAsync(),
            nameof(GetLastRegistroAsync),
            userId
        );
    }

    public async Task<long> CreateRegistroAsync(Registro registro)
    {
        if (registro == null) return 0;

        return await SafeExecuteAsync(async () =>
        {
            await _context.Registros.AddAsync(registro);
            await _context.SaveChangesAsync();
            return registro.Registroid;
        }, nameof(CreateRegistroAsync), registro.Userid);
    }

    public async Task<bool> DeleteRegistroAsync(Registro registro)
    {
        if (registro == null) return false;

        return await SafeExecuteAsync(async () =>
        {
            _context.Registros.Remove(registro);
            await _context.SaveChangesAsync();
            return true;
        }, nameof(DeleteRegistroAsync), registro.Userid);
    }
}
