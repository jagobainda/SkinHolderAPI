using AutoMapper;
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

public class RegistrosLogic(IRegistrosDataService registrosDataService, IMapper mapper, IConfiguration config) : BaseLogic(mapper, config), IRegistrosLogic
{
    private readonly IRegistrosDataService _registrosDataService = registrosDataService;

    public async Task<List<RegistroDto>> GetRegistrosAsync(int userId)
    {
        var registros = await _registrosDataService.GetRegistrosAsync(userId);

        if (registros == null) return [];

        return [.. registros.Select(_mapper.Map<RegistroDto>)];
    }

    public async Task<RegistroDto?> GetRegistroAsync(long registroId)
    {
        var registro = await _registrosDataService.GetRegistroAsync(registroId);

        if (registro == null) return null;

        return _mapper.Map<RegistroDto>(registro);
    }

    public async Task<RegistroDto?> GetLastRegistroAsync(int userId)
    {
        var registro = await _registrosDataService.GetLastRegistroAsync(userId);

        if (registro == null) return null;

        return _mapper.Map<RegistroDto>(registro);
    }

    public async Task<long> CreateRegistroAsync(RegistroDto registroDto)
    {
        var registro = _mapper.Map<Registro>(registroDto);

        registro.Fechahora = DateTime.Now;

        return await _registrosDataService.CreateRegistroAsync(registro);
    }

    public async Task<bool> DeleteRegistroAsync(long registroId)
    {
        var registro = await _registrosDataService.GetRegistroAsync(registroId);

        if (registro == null) return false;

        return await _registrosDataService.DeleteRegistroAsync(registro);
    }

    public async Task<RegistroVarianceStatsDto?> GetVarianceStatsAsync(int userId)
    {
        var registros = await _registrosDataService.GetRegistrosAsync(userId);

        if (registros == null || registros.Count < 2) return null;

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

        return new RegistroVarianceStatsDto
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
    }

    private static double Variance(decimal? oldValue, decimal newValue)
    {
        if (!oldValue.HasValue || oldValue.Value == 0) return -101;
        return (double)(((newValue - oldValue.Value) / oldValue.Value) * 100);
    }
}
