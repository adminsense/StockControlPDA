namespace StockControl.Admin.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "StockControl";
    public string Audience { get; set; } = "StockControl";
    public int ExpiryHours { get; set; } = 12;
}
