using Microsoft.EntityFrameworkCore;

namespace StockControl.Admin.Data;

/// <summary>
/// Latest PDA-driven stock activity (movements + balance updates). Used by the Stock page Sync only —
/// master data is edited in Admin and does not trigger this marker.
/// </summary>
public static class PdaStockSyncMarker
{
    public static async Task<DateTimeOffset> GetLatestActivityUtcAsync(
        AppDbContext db,
        CancellationToken cancellationToken = default)
    {
        var max = DateTimeOffset.MinValue;

        if (await db.StockBalances.AsNoTracking().AnyAsync(cancellationToken))
            max = Max(max, await db.StockBalances.AsNoTracking().MaxAsync(b => b.UpdatedAt ?? b.CreatedAt, cancellationToken));

        if (await db.StockMovements.AsNoTracking().AnyAsync(cancellationToken))
            max = Max(max, await db.StockMovements.AsNoTracking().MaxAsync(m => m.CreatedAt, cancellationToken));

        return max;
    }

    private static DateTimeOffset Max(DateTimeOffset a, DateTimeOffset b) => a >= b ? a : b;
}
