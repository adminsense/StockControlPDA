using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Options;

using StockControl.Admin.Auth;

using StockControl.Admin.Data;



namespace StockControl.Admin.Api;



public sealed record LoginRequest(string Username, string Password);



public sealed record LoginResponse(bool Ok, string? Token, string? DisplayName, string? Error);



public static class AuthApiExtensions

{

    public static WebApplication MapAuthApi(this WebApplication app)

    {

        app.MapPost("/api/auth/login", LoginAsync)

            .WithName("Login")

            .DisableAntiforgery()

            .AllowAnonymous();



        app.MapPost("/api/auth/logout", LogoutAsync)

            .WithName("Logout")

            .DisableAntiforgery()

            .AllowAnonymous();



        return app;

    }



    private static async Task<IResult> LoginAsync(

        LoginRequest body,

        HttpContext http,

        [FromQuery] string? app,

        [FromServices] UserAuthService userAuth,

        [FromServices] JwtTokenService jwt,

        [FromServices] IOptions<AuthOptions> authOptions,

        [FromServices] IAuditService audit,

        CancellationToken cancellationToken)

    {

        var isPda = string.Equals(app, "pda", StringComparison.OrdinalIgnoreCase);

        var requiredRole = isPda ? UserRole.AdminPda : UserRole.Admin;

        var ip = ClientIp(http);

        var username = (body.Username ?? "").Trim();



        var result = await userAuth.ValidateAsync(body.Username, body.Password, requiredRole);

        if (!result.Ok || result.User is null)

        {

            await audit.LogAsync(new AuditEntry(

                AuditActions.LoginFailed,

                "Auth",

                Username: string.IsNullOrEmpty(username) ? "unknown" : username,

                Success: false,

                Severity: "Warning",

                IpAddress: ip,

                ErrorMessage: result.Error ?? "Login failed."), cancellationToken);



            return Results.Json(new LoginResponse(false, null, null, result.Error ?? "Login failed."), statusCode: 401);

        }



        var token = jwt.CreateToken(result.User);



        await audit.LogAsync(new AuditEntry(

            AuditActions.Login,

            "Auth",

            EntityId: result.User.Id,

            UserId: result.User.Id,

            Username: result.User.Username,

            IpAddress: ip,

            NewValues: isPda ? """{"app":"pda"}""" : """{"app":"admin"}"""), cancellationToken);



        if (isPda)

            return Results.Ok(new LoginResponse(true, token, result.User.Name, null));



        AuthCookieHelper.AppendJwtCookie(http, token, authOptions.Value);

        // Token in JSON so Blazor can update AuthenticationState without losing auth on NavLink clicks.
        return Results.Ok(new LoginResponse(true, token, result.User.Name, null));

    }



    private static async Task<IResult> LogoutAsync(

        HttpContext http,

        [FromServices] IAuditService audit,

        CancellationToken cancellationToken)

    {

        var ip = ClientIp(http);

        var userIdClaim = http.User.FindFirstValue(AuthConstants.UserIdClaim)

            ?? http.User.FindFirstValue(ClaimTypes.NameIdentifier);

        int? userId = int.TryParse(userIdClaim, out var uid) ? uid : null;

        var username = http.User.Identity?.Name ?? "unknown";



        if (userId is not null || http.User.Identity?.IsAuthenticated == true)

        {

            await audit.LogAsync(new AuditEntry(

                AuditActions.Logout,

                "Auth",

                EntityId: userId,

                UserId: userId,

                Username: username,

                IpAddress: ip), cancellationToken);

        }



        AuthCookieHelper.DeleteJwtCookie(http);

        return Results.Ok();

    }



    private static string? ClientIp(HttpContext http)

        => http.Connection.RemoteIpAddress?.ToString();

}


