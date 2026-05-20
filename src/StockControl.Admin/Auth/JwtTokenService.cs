using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StockControl.Admin.Data;

namespace StockControl.Admin.Auth;

public sealed class JwtTokenService
{
    private readonly AuthOptions _options;
    private readonly TokenValidationParameters _validationParameters;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtTokenService(IOptions<AuthOptions> options)
    {
        _options = options.Value;
        _validationParameters = AuthJwt.CreateValidationParameters(_options);
    }

    public string CreateToken(AppUser user)
    {
        var key = AuthJwt.CreateSigningKey(_options);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var principal = UserAuthService.CreatePrincipal(user);
        var expires = DateTime.UtcNow.AddHours(Math.Max(1, _options.ExpiryHours));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: principal.Claims,
            expires: expires,
            signingCredentials: creds);

        return _handler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            return _handler.ValidateToken(token, _validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}
