using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using StockControl.PDA.Configuration;

namespace StockControl.PDA.Services;

public interface IMoveStockCatalogClient
{
    Task<IReadOnlyList<CatalogWarehouse>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CatalogLocation>> GetLocationsAsync(int warehouseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CatalogItem>> GetItemsAsync(CancellationToken cancellationToken = default);
    Task<CatalogSummary?> GetSummaryAsync(int warehouseId, int locationId, int itemId, CancellationToken cancellationToken = default);
}

public sealed class CatalogWarehouse
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string PickerText => $"{Code} — {Name}";
    public override string ToString() => PickerText;
}

public sealed class CatalogLocation
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
    public string PickerText => string.IsNullOrWhiteSpace(Description) ? Code : $"{Code} — {Description}";
    public override string ToString() => PickerText;
}

public sealed class CatalogItem
{
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string ArticleNumber { get; set; } = "";
    public string Barcodes { get; set; } = "";
    public string PickerText => $"{Sku} — {DisplayName}";
    public override string ToString() => PickerText;
}

public sealed class CatalogSummary
{
    public string WarehouseCode { get; set; } = "";
    public string WarehouseName { get; set; } = "";
    public string LocationCode { get; set; } = "";
    public string ItemSku { get; set; } = "";
    public string ItemName { get; set; } = "";
    public decimal OnHand { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public string Status { get; set; } = "";
}

public sealed class MoveStockCatalogHttpClient : IMoveStockCatalogClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient _http;
    private readonly IOptions<ApiOptions> _options;

    public MoveStockCatalogHttpClient(HttpClient http, IOptions<ApiOptions> options)
    {
        _http = http;
        _options = options;
    }

    private bool EnsureClient(out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(_options.Value.BaseUrl?.Trim()))
        {
            error = "API base URL is not configured.";
            return false;
        }

        if (_http.BaseAddress is null)
        {
            error = "HTTP client has no base address.";
            return false;
        }

        return true;
    }

    public async Task<IReadOnlyList<CatalogWarehouse>> GetWarehousesAsync(CancellationToken cancellationToken = default)
    {
        if (!EnsureClient(out _))
            return [];
        try
        {
            var list = await _http.GetFromJsonAsync<List<CatalogWarehouse>>("api/pda/catalog/warehouses", JsonOptions, cancellationToken);
            return list ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<IReadOnlyList<CatalogLocation>> GetLocationsAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (!EnsureClient(out _))
            return [];
        try
        {
            var list = await _http.GetFromJsonAsync<List<CatalogLocation>>(
                $"api/pda/catalog/warehouses/{warehouseId}/locations", JsonOptions, cancellationToken);
            return list ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<IReadOnlyList<CatalogItem>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        if (!EnsureClient(out _))
            return [];
        try
        {
            var list = await _http.GetFromJsonAsync<List<CatalogItem>>("api/pda/catalog/items", JsonOptions, cancellationToken);
            return list ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<CatalogSummary?> GetSummaryAsync(int warehouseId, int locationId, int itemId, CancellationToken cancellationToken = default)
    {
        if (!EnsureClient(out _))
            return null;
        try
        {
            return await _http.GetFromJsonAsync<CatalogSummary>(
                $"api/pda/catalog/summary?warehouseId={warehouseId}&locationId={locationId}&itemId={itemId}",
                JsonOptions,
                cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
