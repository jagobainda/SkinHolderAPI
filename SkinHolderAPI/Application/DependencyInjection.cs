using SkinHolderAPI.Application.Apis;
using SkinHolderAPI.Application.Background;
using SkinHolderAPI.Application.External;
using SkinHolderAPI.Application.ItemPrecio;
using SkinHolderAPI.Application.Items;
using SkinHolderAPI.Application.Loggers;
using SkinHolderAPI.Application.Login;
using SkinHolderAPI.Application.Registros;
using SkinHolderAPI.Application.Security;
using SkinHolderAPI.Application.UserItems;
using SkinHolderAPI.Application.Users;
using SkinHolderAPI.DataService.Apis;
using SkinHolderAPI.DataService.External;
using SkinHolderAPI.DataService.ItemPrecio;
using SkinHolderAPI.DataService.Items;
using SkinHolderAPI.DataService.Log;
using SkinHolderAPI.DataService.Registros;
using SkinHolderAPI.DataService.UserItems;
using SkinHolderAPI.DataService.Users;

namespace SkinHolderAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // DataServives
        services.AddScoped<ILogDataService, LogDataService>();
        services.AddScoped<IUserDataService, UserDataService>();
        services.AddScoped<IApiQueryDataService, ApiQueryDataService>();
        services.AddScoped<IItemsDataService, ItemsDataService>();
        services.AddScoped<IUserItemsDataService, UserItemsDataService>();
        services.AddScoped<IRegistrosDataService, RegistrosDataService>();
        services.AddScoped<IItemPrecioDataService, ItemPrecioDataService>();

        services.AddScoped<IExternalDataService, ExternalDataService>();

        // BusinessLogics
        services.AddScoped<ILogLogic, LogLogic>();
        services.AddScoped<ITokenLogic, TokenLogic>();
        services.AddSingleton<IRateLimitLogic, RateLimitLogic>();
        services.AddScoped<IUserLogic, UserLogic>();
        services.AddScoped<IApiQueryLogic, ApiQueryLogic>();
        services.AddScoped<IItemsLogic, ItemsLogic>();
        services.AddScoped<IUserItemsLogic, UserItemsLogic>();
        services.AddScoped<IRegistrosLogic, RegistrosLogic>();
        services.AddScoped<IItemPrecioLogic, ItemPrecioLogic>();

        services.AddScoped<IExternalLogic, ExternalLogic>();

        // Steam Queue Service
        services.AddSingleton<SteamPriceQueueService>();
        services.AddSingleton<ISteamPriceQueueService>(sp => sp.GetRequiredService<SteamPriceQueueService>());

        // HttpClient para Steam API
        services.AddHttpClient("SteamAPI", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("User-Agent", "SkinHolderAPI/1.0");
        });

        // Singletons
        services.AddSingleton<TokenLogic>();

        // Background Services
        services.AddHostedService<LogCleanupService>();
        services.AddHostedService<SteamPriceWorker>();

        // Mapper
        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
}
