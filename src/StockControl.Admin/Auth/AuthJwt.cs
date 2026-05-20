using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace StockControl.Admin.Auth;

internal static class AuthJwt
{
    public static SymmetricSecurityKey CreateSigningKey(AuthOptions options)
        => new(Encoding.UTF8.GetBytes(options.Key));

    public static TokenValidationParameters CreateValidationParameters(AuthOptions options)
    {
        var key = CreateSigningKey(options);
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    }
}
