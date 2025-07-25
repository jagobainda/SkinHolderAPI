﻿using AutoMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.Registros;
using SkinHolderAPI.DTOs.Registros;

namespace SkinHolderAPI.Application.Registros;

public interface IRegistrosLogic
{
    Task<List<RegistroDto>> GetRegistrosAsync(int userId);
    Task<RegistroDto?> GetRegistroAsync(long registroId);
    Task<RegistroDto?> GetLastRegistroAsync(int userId);
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
}
