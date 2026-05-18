namespace StockControl.Admin.Validation;

public readonly record struct StockFilterRow(
    int WarehouseId,
    int LocationId,
    int SupplierId,
    string SupplierCode,
    string SupplierName,
    string ProductCode,
    string Sku,
    string ArticleNumber,
    string DisplayName,
    decimal QuantityOnHand,
    int? Min,
    int? Max);

public static class StockGridFilter
{
    public static bool Matches(
        StockFilterRow r,
        int warehouseId,
        int locationId,
        int supplierId,
        string search,
        string filter)
    {
        if (warehouseId != 0 && r.WarehouseId != warehouseId)
            return false;
        if (locationId != 0 && r.LocationId != locationId)
            return false;
        if (supplierId != 0 && r.SupplierId != supplierId)
            return false;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var matches =
                r.Sku.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(r.ArticleNumber)
                    && r.ArticleNumber.Contains(search, StringComparison.OrdinalIgnoreCase))
                || r.ProductCode.Contains(search, StringComparison.OrdinalIgnoreCase)
                || r.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase);
            if (!matches)
                return false;
        }

        return filter switch
        {
            "below" => r.Min is not null && r.QuantityOnHand < r.Min,
            "above" => r.Max is not null && r.QuantityOnHand > r.Max,
            _ => true
        };
    }

    /// <summary>Quantity to bring on-hand up to min when below threshold; otherwise null.</summary>
    public static decimal? ReorderQuantity(StockFilterRow r)
    {
        if (r.Min is null || r.QuantityOnHand >= r.Min)
            return null;
        return r.Min.Value - r.QuantityOnHand;
    }
}

public static class StockGridPaging
{
    public static int MaxPageIndex(int totalRows, int pageSize) =>
        Math.Max(0, (totalRows - 1) / pageSize);

    public static int ShowFrom(int totalRows, int pageIndex, int pageSize) =>
        totalRows == 0 ? 0 : (pageIndex * pageSize) + 1;

    public static int ShowTo(int totalRows, int pageIndex, int pageSize) =>
        Math.Min(totalRows, (pageIndex * pageSize) + pageSize);
}
