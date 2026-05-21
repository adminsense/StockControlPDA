namespace StockControl.Admin.Auth;

public static class AuthCookieHelper
{
    public static void AppendJwtCookie(HttpContext http, string token, AuthOptions options)
    {
        var hours = Math.Max(1, options.ExpiryHours);
        http.Response.Cookies.Append(AuthConstants.JwtCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = http.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            MaxAge = TimeSpan.FromHours(hours),
            IsEssential = true
        });
    }

    public static void DeleteJwtCookie(HttpContext http)
        => http.Response.Cookies.Delete(AuthConstants.JwtCookieName, new CookieOptions { Path = "/" });
}
