using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace StockControl.Admin.Auth;

public sealed class ServerAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
        return Task.FromResult(new AuthenticationState(user));
    }

    public void NotifyAuthenticationStateChanged()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
