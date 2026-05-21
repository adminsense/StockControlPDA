using System.Text.Json.Serialization;

namespace StockControl.Admin.Auth;

/// <summary>Matches JSON returned by <c>loginAdmin</c> in stockcontrol.js.</summary>
public sealed class LoginJsResult
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
