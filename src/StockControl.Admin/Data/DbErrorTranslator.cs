using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace StockControl.Admin.Data;

public static class DbErrorTranslator
{
    public static bool TryGetUserFriendlyMessage(Exception ex, out string message)
    {
        message = "";

        var sql = FindSqlException(ex);
        if (sql is not null)
        {
            // 2601 = Cannot insert duplicate key row
            // 2627 = Violation of UNIQUE KEY constraint
            if (sql.Number is 2601 or 2627)
            {
                var raw = sql.Message ?? "";
                message = raw switch
                {
                    var m when m.Contains("IX_users_Username", StringComparison.OrdinalIgnoreCase)
                        => "Username already exists.",
                    var m when m.Contains("IX_warehouses_Code", StringComparison.OrdinalIgnoreCase)
                        => "Warehouse code already exists.",
                    var m when m.Contains("IX_locations_WarehouseId_Code", StringComparison.OrdinalIgnoreCase)
                        => "Location code already exists for this warehouse.",
                    var m when m.Contains("IX_suppliers_Code", StringComparison.OrdinalIgnoreCase)
                        => "Supplier code already exists.",
                    var m when m.Contains("IX_products_Code", StringComparison.OrdinalIgnoreCase)
                        => "Product code already exists.",
                    var m when m.Contains("IX_items_Sku", StringComparison.OrdinalIgnoreCase)
                        => "SKU already exists.",
                    var m when m.Contains("IX_item_barcodes_Code", StringComparison.OrdinalIgnoreCase)
                        => "Barcode already exists (it must be unique).",
                    var m when m.Contains("IX_minmax_settings_WarehouseId_ItemId_LocationId", StringComparison.OrdinalIgnoreCase)
                        => "A Min/Max setting already exists for this warehouse + item + location.",
                    var m when m.Contains("IX_minmax_settings_WarehouseId_ItemId", StringComparison.OrdinalIgnoreCase)
                        => "A Min/Max setting already exists for this warehouse + item (without location).",
                    _ => "Duplicate value. Please review the fields and try again."
                };
                return true;
            }
        }

        if (ex is DbUpdateException)
        {
            message = "Could not save changes due to a database constraint. Please review the fields and try again.";
            return true;
        }

        return false;
    }

    private static SqlException? FindSqlException(Exception ex)
    {
        for (var cur = ex; cur is not null; cur = cur.InnerException!)
        {
            if (cur is SqlException sqlEx)
                return sqlEx;
        }

        return null;
    }
}

