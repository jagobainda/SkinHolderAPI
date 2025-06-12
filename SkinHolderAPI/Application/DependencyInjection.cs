using SkinHolderAPI.Application.Login;
using SkinHolderAPI.Application.Security;
using SkinHolderAPI.Application.Users;
using SkinHolderAPI.DataService.Users;

namespace SkinHolderAPI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // DataServives
            services.AddScoped<IUserDataService, UserDataService>();

            // BusinessLogics
            services.AddScoped<ITokenLogic, TokenLogic>();
            services.AddSingleton<IRateLimitLogic, RateLimitLogic>();
            services.AddScoped<IUserLogic, UserLogic>();

            // Singletons
            services.AddSingleton<TokenLogic>();

            // Mapper
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
