namespace StockControl.Admin.Auth;

public static class AuthConstants
{
    public const string AdminRole = "Admin";
    public const string AdminPdaRole = "AdminPda";
    public const string UserIdClaim = "uid";
    /// <summary>HttpOnly cookie holding the JWT for Blazor Admin (same token format as PDA Bearer).</summary>
    public const string JwtCookieName = "StockControl.Jwt";
}
