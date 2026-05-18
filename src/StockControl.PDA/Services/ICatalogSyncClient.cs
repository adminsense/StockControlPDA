namespace StockControl.PDA.Services;

public sealed record CatalogSyncResult(bool Success, string Message);

public interface ICatalogSyncClient
{
    Task<CatalogSyncResult> SyncAsync(CancellationToken cancellationToken = default);
}
