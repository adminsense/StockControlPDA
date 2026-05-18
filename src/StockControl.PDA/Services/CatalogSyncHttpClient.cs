using Microsoft.Extensions.Options;
using StockControl.PDA.Configuration;

namespace StockControl.PDA.Services;

public sealed class CatalogSyncHttpClient : ICatalogSyncClient
{
    private readonly HttpClient _http;
    private readonly IOptions<ApiOptions> _options;

    public CatalogSyncHttpClient(HttpClient http, IOptions<ApiOptions> options)
    {
        _http = http;
        _options = options;
    }

    public async Task<CatalogSyncResult> SyncAsync(CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.Value.BaseUrl?.Trim();
        if (string.IsNullOrWhiteSpace(baseUrl))
            return new CatalogSyncResult(false, "API base URL is not configured. Set Api:BaseUrl in appsettings.json.");

        if (_http.BaseAddress is null)
            return new CatalogSyncResult(false, "HTTP client has no base address. Check Api:BaseUrl format.");

        try
        {
            var response = await _http.GetAsync("api/stock/sync", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync(cancellationToken);
                var detail = string.IsNullOrWhiteSpace(err) ? response.ReasonPhrase ?? "Unknown error" : err.Trim().Trim('"');
                return new CatalogSyncResult(false, detail);
            }

            try
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var doc = await System.Text.Json.JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
                var root = doc.RootElement;
                var wh = root.TryGetProperty("warehouseCount", out var w) && w.TryGetInt32(out var wi) ? wi : 0;
                var loc = root.TryGetProperty("locationCount", out var l) && l.TryGetInt32(out var li) ? li : 0;
                var it = root.TryGetProperty("itemCount", out var i) && i.TryGetInt32(out var ii) ? ii : 0;
                return new CatalogSyncResult(true, $"Synced. Warehouses: {wh}, locations: {loc}, items: {it}.");
            }
            catch
            {
                return new CatalogSyncResult(true, "Synced.");
            }
        }
        catch (Exception ex)
        {
            return new CatalogSyncResult(false, ex.Message);
        }
    }
}
