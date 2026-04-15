using MapsterMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.Registros;
using SkinHolderAPI.DTOs.Registros;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.Application.Registros;

public interface IRegistrosLogic
{
    Task<List<RegistroDto>> GetRegistrosAsync(int userId);
    Task<RegistroDto?> GetRegistroAsync(long registroId);
    Task<RegistroDto?> GetLastRegistroAsync(int userId);
    Task<long> CreateRegistroAsync(RegistroDto registroDto);
    Task<bool> DeleteRegistroAsync(long registroId);
    Task<RegistroVarianceStatsDto?> GetVarianceStatsAsync(int userId);
}

public class RegistrosLogic(IRegistrosDataService registrosDataService, IMapper mapper, IConfiguration config, ILogger<RegistrosLogic> logger) : BaseLogic(mapper, config, logger), IRegistrosLogic
{
    private readonly IRegistrosDataService _registrosDataService = registrosDataService;

    public async Task<List<RegistroDto>> GetRegistrosAsync(int userId)
    {
        _logger.LogInformation("GetRegistrosAsync llamado para userId={UserId}", userId);

        try
        {
            var registros = await _registrosDataService.GetRegistrosAsync(userId);

            if (registros == null || registros.Count == 0)
            {
                _logger.LogWarning("GetRegistrosAsync no devolvió registros para userId={UserId}", userId);
                return [];
            }

            var result = registros.Select(r => _mapper.Map<RegistroDto>(r)).ToList();

            _logger.LogInformation("GetRegistrosAsync completado: {Count} registros para userId={UserId}", result.Count, userId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetRegistrosAsync para userId={UserId}", userId);
            throw;
        }
    }

    public async Task<RegistroDto?> GetRegistroAsync(long registroId)
    {
        _logger.LogInformation("GetRegistroAsync llamado para registroId={RegistroId}", registroId);

        try
        {
            var registro = await _registrosDataService.GetRegistroAsync(registroId);

            if (registro == null)
            {
                _logger.LogWarning("GetRegistroAsync: registro no encontrado para registroId={RegistroId}", registroId);
                return null;
            }

            _logger.LogInformation("GetRegistroAsync completado para registroId={RegistroId}", registroId);

            return _mapper.Map<RegistroDto>(registro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetRegistroAsync para registroId={RegistroId}", registroId);
            throw;
        }
    }

    public async Task<RegistroDto?> GetLastRegistroAsync(int userId)
    {
        _logger.LogInformation("GetLastRegistroAsync llamado para userId={UserId}", userId);

        try
        {
            var registro = await _registrosDataService.GetLastRegistroAsync(userId);

            if (registro == null)
            {
                _logger.LogWarning("GetLastRegistroAsync: no se encontró último registro para userId={UserId}", userId);
                return null;
            }

            _logger.LogInformation("GetLastRegistroAsync completado para userId={UserId}, registroId={RegistroId}", userId, registro.Registroid);

            return _mapper.Map<RegistroDto>(registro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetLastRegistroAsync para userId={UserId}", userId);
            throw;
        }
    }

    public async Task<long> CreateRegistroAsync(RegistroDto registroDto)
    {
        _logger.LogInformation("CreateRegistroAsync llamado para userId={UserId}", registroDto.Userid);

        try
        {
            var registro = _mapper.Map<Registro>(registroDto);
            registro.Fechahora = DateTime.Now;

            var registroId = await _registrosDataService.CreateRegistroAsync(registro);

            if (registroId == 0)
                _logger.LogWarning("CreateRegistroAsync falló al crear registro para userId={UserId}", registroDto.Userid);
            else
                _logger.LogInformation("CreateRegistroAsync completado: registroId={RegistroId} creado para userId={UserId}", registroId, registroDto.Userid);

            return registroId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateRegistroAsync para userId={UserId}", registroDto.Userid);
            throw;
        }
    }

    public async Task<bool> DeleteRegistroAsync(long registroId)
    {
        _logger.LogInformation("DeleteRegistroAsync llamado para registroId={RegistroId}", registroId);

        try
        {
            var registro = await _registrosDataService.GetRegistroAsync(registroId);

            if (registro == null)
            {
                _logger.LogWarning("DeleteRegistroAsync: registro no encontrado para registroId={RegistroId}", registroId);
                return false;
            }

            var result = await _registrosDataService.DeleteRegistroAsync(registro);

            if (!result)
                _logger.LogWarning("DeleteRegistroAsync falló al eliminar registroId={RegistroId}", registroId);
            else
                _logger.LogInformation("DeleteRegistroAsync completado: registroId={RegistroId} eliminado", registroId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en DeleteRegistroAsync para registroId={RegistroId}", registroId);
            throw;
        }
    }

    public async Task<RegistroVarianceStatsDto?> GetVarianceStatsAsync(int userId)
    {
        _logger.LogInformation("GetVarianceStatsAsync llamado para userId={UserId}", userId);

        try
        {
            var registros = await _registrosDataService.GetRegistrosAsync(userId);

            if (registros == null || registros.Count < 2)
            {
                _logger.LogWarning("GetVarianceStatsAsync: datos insuficientes para userId={UserId} (registros={Count})", userId, registros?.Count ?? 0);
                return null;
            }

            var sorted = registros.OrderByDescending(r => r.Fechahora).ToList();
            var current = sorted[0];
            var now = DateTime.Now;

            DateTime weekAgo = now.AddDays(-7);
            DateTime monthAgo = now.AddMonths(-1);
            DateTime yearAgo = now.AddYears(-1);

            Registro? week = null;
            Registro? month = null;
            Registro? year = null;

            foreach (var r in sorted)
            {
                if (week == null && r.Fechahora <= weekAgo) week = r;
                if (month == null && r.Fechahora <= monthAgo) month = r;
                if (year == null && r.Fechahora <= yearAgo) year = r;

                if (week != null && month != null && year != null) break;
            }

            var result = new RegistroVarianceStatsDto
            {
                WeeklyVariancePercentSteam = Variance(week?.Totalsteam, current.Totalsteam),
                MonthlyVariancePercentSteam = Variance(month?.Totalsteam, current.Totalsteam),
                YearlyVariancePercentSteam = Variance(year?.Totalsteam, current.Totalsteam),

                WeeklyVariancePercentGamerPay = Variance(week?.Totalgamerpay, current.Totalgamerpay),
                MonthlyVariancePercentGamerPay = Variance(month?.Totalgamerpay, current.Totalgamerpay),
                YearlyVariancePercentGamerPay = Variance(year?.Totalgamerpay, current.Totalgamerpay),

                WeeklyVariancePercentCSFloat = Variance(week?.Totalcsfloat, current.Totalcsfloat),
                MonthlyVariancePercentCSFloat = Variance(month?.Totalcsfloat, current.Totalcsfloat),
                YearlyVariancePercentCSFloat = Variance(year?.Totalcsfloat, current.Totalcsfloat)
            };

            _logger.LogInformation("GetVarianceStatsAsync completado para userId={UserId} con {Count} registros analizados", userId, sorted.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetVarianceStatsAsync para userId={UserId}", userId);
            throw;
        }
    }

    private static double Variance(decimal? oldValue, decimal newValue)
    {
        if (!oldValue.HasValue || oldValue.Value == 0) return -101;
        return (double)(((newValue - oldValue.Value) / oldValue.Value) * 100);
    }
}
