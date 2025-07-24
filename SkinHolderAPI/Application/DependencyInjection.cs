using SkinHolderAPI.Application.Apis;
using SkinHolderAPI.Application.Items;
using SkinHolderAPI.Application.Login;
using SkinHolderAPI.Application.Registros;
using SkinHolderAPI.Application.Security;
using SkinHolderAPI.Application.Users;
using SkinHolderAPI.DataService.Apis;
using SkinHolderAPI.DataService.Items;
using SkinHolderAPI.DataService.Registros;
using SkinHolderAPI.DataService.Users;

namespace SkinHolderAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // DataServives
        services.AddScoped<IUserDataService, UserDataService>();
        services.AddScoped<IApiQueryDataService, ApiQueryDataService>();
        services.AddScoped<IItemsDataService, ItemsDataService>();
        services.AddScoped<IRegistrosDataService, RegistrosDataService>();

        // BusinessLogics
        services.AddScoped<ITokenLogic, TokenLogic>();
        services.AddSingleton<IRateLimitLogic, RateLimitLogic>();
        services.AddScoped<IUserLogic, UserLogic>();
        services.AddScoped<IApiQueryLogic, ApiQueryLogic>();
        services.AddScoped<IItemsLogic, ItemsLogic>();
        services.AddScoped<IRegistrosLogic, RegistrosLogic>();

        // Singletons
        services.AddSingleton<TokenLogic>();

        // Mapper
        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
}
