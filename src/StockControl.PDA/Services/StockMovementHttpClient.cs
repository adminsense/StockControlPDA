using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using StockControl.PDA.Configuration;

namespace StockControl.PDA.Services;

public sealed class StockMovementHttpClient : IStockMovementClient
{
    private readonly HttpClient _http;
    private readonly IOptions<ApiOptions> _options;

    public StockMovementHttpClient(HttpClient http, IOptions<ApiOptions> options)
    {
        _http = http;
        _options = options;
    }

    public async Task<MovementSubmitResult> SubmitAsync(StockMovementRequest request, CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.Value.BaseUrl?.Trim();
        if (string.IsNullOrWhiteSpace(baseUrl))
            return MovementSubmitResult.Fail("API base URL is not configured. Set Api:BaseUrl in appsettings.json.");

        if (_http.BaseAddress is null)
            return MovementSubmitResult.Fail("HTTP client has no base address. Check Api:BaseUrl format.");

        try
        {
            var response = await _http.PostAsJsonAsync("api/stock/movements", request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var body = await response.Content.ReadAsStringAsync(cancellationToken);
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(body);
                        if (doc.RootElement.TryGetProperty("message", out var m) && m.ValueKind == System.Text.Json.JsonValueKind.String)
                            return MovementSubmitResult.Ok(m.GetString() ?? "Saved.");
                    }
                }
                catch
                {
                    // ignore malformed success body
                }

                return MovementSubmitResult.Ok();
            }

            var errBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var detail = string.IsNullOrWhiteSpace(errBody) ? response.ReasonPhrase ?? "Unknown error" : errBody.Trim().Trim('"');
            return MovementSubmitResult.Fail(detail);
        }
        catch (Exception ex)
        {
            return MovementSubmitResult.Fail(ex.Message);
        }
    }
}
