using Microsoft.AspNetCore.Mvc;
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
        return app;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest body,
        [FromQuery] string? app,
        [FromServices] UserAuthService userAuth,
        [FromServices] JwtTokenService jwt,
        CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        var requiredRole = string.Equals(app, "pda", StringComparison.OrdinalIgnoreCase)
            ? UserRole.AdminPda
            : UserRole.Admin;

        var result = await userAuth.ValidateAsync(body.Username, body.Password, requiredRole);
        if (!result.Ok || result.User is null)
            return Results.Json(new LoginResponse(false, null, null, result.Error ?? "Login failed."), statusCode: 401);

        var token = jwt.CreateToken(result.User);
        return Results.Ok(new LoginResponse(true, token, result.User.Name, null));
    }
}
