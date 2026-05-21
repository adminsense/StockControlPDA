using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace StockControl.Admin.Auth;

/// <summary>
/// Blazor Server: HttpContext is only reliable on the initial HTTP request.
/// Cache the principal for the circuit lifetime and fall back to the JWT cookie.
/// </summary>
public sealed class ServerAuthenticationStateProvider(
    IHttpContextAccessor httpContextAccessor,
    JwtTokenService jwt) : AuthenticationStateProvider
{
    private ClaimsPrincipal? _cached;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = ResolvePrincipal();
        return Task.FromResult(new AuthenticationState(principal));
    }

    /// <summary>After browser login (token from API); avoids forceLoad loop on menu navigation.</summary>
    public void SetAuthenticatedFromToken(string token)
    {
        var principal = jwt.ValidateToken(token);
        _cached = principal?.Identity?.IsAuthenticated == true
            ? principal
            : new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SignOut()
    {
        _cached = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private ClaimsPrincipal ResolvePrincipal()
    {
        if (_cached?.Identity?.IsAuthenticated == true)
            return _cached;

        var http = httpContextAccessor.HttpContext;

        if (http?.User?.Identity?.IsAuthenticated == true)
        {
            _cached = http.User;
            return _cached;
        }

        if (http?.Request.Cookies.TryGetValue(AuthConstants.JwtCookieName, out var cookieToken) == true
            && !string.IsNullOrWhiteSpace(cookieToken))
        {
            var fromCookie = jwt.ValidateToken(cookieToken);
            if (fromCookie?.Identity?.IsAuthenticated == true)
            {
                _cached = fromCookie;
                return _cached;
            }
        }

        _cached = new ClaimsPrincipal(new ClaimsIdentity());
        return _cached;
    }
}
