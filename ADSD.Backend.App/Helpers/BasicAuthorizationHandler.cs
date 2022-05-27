using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ADSD.Backend.App.Helpers;

public class BasicAuthorizationHandler : AuthorizationHandler<BasicAuthorizationRequirement>
{
    private readonly IMemoryCache _memoryCache;
    private readonly SessionTokenDbClient _sessionTokenDbClient;

    public BasicAuthorizationHandler(IMemoryCache memoryCache, SessionTokenDbClient sessionTokenDbClient)
    {
        _memoryCache = memoryCache;
        _sessionTokenDbClient = sessionTokenDbClient;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BasicAuthorizationRequirement requirement)
    {
        if (context.Resource is AuthorizationFilterContext mvcContext)
        {
            var httpContext = mvcContext.HttpContext;

            if (!_memoryCache.TryGetValue(httpContext.Request.Headers.Authorization, out AuthData authData))
            {
                authData = await _sessionTokenDbClient.GetUserByToken(httpContext.Request.Headers.Authorization);
            }

            if (authData != null)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}