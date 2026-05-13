using Microsoft.EntityFrameworkCore;
using StockControl.Admin.Validation;

namespace StockControl.Admin.Data;

public static class ItemBarcodeConflictFinder
{
    public static async Task<string?> FindConflictAsync(
        AppDbContext db,
        int editingItemId,
        IReadOnlyList<string> incomingCodes,
        CancellationToken cancellationToken = default)
    {
        if (incomingCodes.Count == 0)
            return null;

        var incoming = incomingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var others = await db.Items
            .AsNoTracking()
            .Where(i => i.Id != editingItemId)
            .Select(i => new { i.Id, i.Barcodes })
            .ToListAsync(cancellationToken);

        foreach (var row in others)
        {
            foreach (var existing in ItemBarcodeInput.ParseLines(row.Barcodes))
            {
                if (incoming.Contains(existing))
                    return $"Barcode \"{existing}\" is already used on another item.";
            }
        }

        return null;
    }
}
