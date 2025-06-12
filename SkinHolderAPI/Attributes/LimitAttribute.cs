using Microsoft.AspNetCore.Mvc.Filters;
using SkinHolderAPI.Application.Security;

namespace SkinHolderAPI.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LimitAttribute(int limit) : Attribute, IAsyncActionFilter
{
    private readonly int _limit = limit;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var rateLimitService = httpContext.RequestServices.GetRequiredService<IRateLimitLogic>();

        var endpointKey = $"{context.ActionDescriptor.DisplayName}:{ipAddress}";

        if (!await rateLimitService.IsAllowedAsync(endpointKey, _limit))
        {
            httpContext.Response.StatusCode = 429;
            await httpContext.Response.WriteAsync("Too many requests. Please try again later.");
            return;
        }

        await next();
    }
}