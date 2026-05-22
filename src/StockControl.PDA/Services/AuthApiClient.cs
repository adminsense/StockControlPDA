using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using StockControl.PDA.Configuration;

namespace StockControl.PDA.Services;

public interface IAuthApiClient
{
    Task<(bool Ok, string? Token, string? DisplayName, string? Error)> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}

public sealed class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _http;
    private readonly IOptions<ApiOptions> _options;

    public AuthApiClient(HttpClient http, IOptions<ApiOptions> options)
    {
        _http = http;
        _options = options;
    }

    public async Task<(bool Ok, string? Token, string? DisplayName, string? Error)> LoginAsync(
        string username, string password, CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.Value.BaseUrl?.Trim();
        if (string.IsNullOrWhiteSpace(baseUrl))
            return (false, null, null, "API base URL is not configured.");

        if (_http.BaseAddress is null)
            return (false, null, null, "HTTP client has no base address.");

        try
        {
            var response = await _http.PostAsJsonAsync(
                "api/auth/login?app=pda",
                new LoginBody(username, password),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var err = await TryReadErrorAsync(response, cancellationToken);
                return (false, null, null, err ?? "Login failed.");
            }

            var body = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
            if (body is null || !body.Ok || string.IsNullOrWhiteSpace(body.Token))
                return (false, null, null, body?.Error ?? "Login failed.");

            return (true, body.Token, body.DisplayName, null);
        }
        catch (Exception ex)
        {
            return (false, null, null, ex.Message);
        }
    }

    private static async Task<string?> TryReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var body = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
            return body?.Error;
        }
        catch
        {
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }

    private sealed record LoginBody(
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("password")] string Password);

    private sealed record LoginResponse(
        [property: JsonPropertyName("ok")] bool Ok,
        [property: JsonPropertyName("token")] string? Token,
        [property: JsonPropertyName("displayName")] string? DisplayName,
        [property: JsonPropertyName("error")] string? Error);
}
