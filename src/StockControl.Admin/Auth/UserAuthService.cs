using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using StockControl.Admin.Data;

namespace StockControl.Admin.Auth;

public sealed record AuthUserResult(bool Ok, AppUser? User, string? Error)
{
    public static AuthUserResult Success(AppUser user) => new(true, user, null);
    public static AuthUserResult Fail(string error) => new(false, null, error);
}

public sealed class UserAuthService(
    IDbContextFactory<AppDbContext> dbFactory,
    PasswordHasherService passwordHasher)
{
    public async Task<AuthUserResult> ValidateAsync(string username, string password, UserRole requiredRole)
    {
        var normalized = (username ?? "").Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return AuthUserResult.Fail("Username is required.");
        if (string.IsNullOrEmpty(password))
            return AuthUserResult.Fail("Password is required.");

        await using var db = await dbFactory.CreateDbContextAsync();
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Username == normalized && u.IsActive);

        if (user is null)
            return AuthUserResult.Fail("Invalid username or password.");
        if (user.Role != requiredRole)
            return AuthUserResult.Fail("This account cannot sign in here.");
        if (!passwordHasher.Verify(user, password))
            return AuthUserResult.Fail("Invalid username or password.");

        return AuthUserResult.Success(user);
    }

    public static ClaimsPrincipal CreatePrincipal(AppUser user)
    {
        var role = user.Role == UserRole.Admin ? AuthConstants.AdminRole : AuthConstants.AdminPdaRole;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(AuthConstants.UserIdClaim, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("display_name", user.Name),
            new(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, authenticationType: "StockControl");
        return new ClaimsPrincipal(identity);
    }
}
