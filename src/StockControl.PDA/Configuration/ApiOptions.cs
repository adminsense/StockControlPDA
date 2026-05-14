namespace StockControl.PDA.Configuration;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>Root URL of the Stock Control API (HTTPS). Leave empty until the API is available.</summary>
    public string BaseUrl { get; set; } = "";
}
