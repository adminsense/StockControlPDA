using Microsoft.AspNetCore.Identity;
using StockControl.Admin.Data;

namespace StockControl.Admin.Auth;

public sealed class PasswordHasherService
{
    private readonly PasswordHasher<AppUser> _hasher = new();

    public string Hash(AppUser user, string password) => _hasher.HashPassword(user, password);

    public bool Verify(AppUser user, string password)
    {
        if (string.IsNullOrEmpty(user.PasswordHash))
            return false;
        return _hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
    }
}
