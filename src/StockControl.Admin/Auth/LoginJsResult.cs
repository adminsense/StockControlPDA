using System.Text.Json.Serialization;

namespace StockControl.Admin.Auth;

/// <summary>Matches JSON returned by <c>loginAdmin</c> in stockcontrol.js.</summary>
public sealed class LoginJsResult
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
