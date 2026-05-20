using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControl.Admin.Data;

namespace StockControl.Admin.Api;

/// <summary>Read-only catalog endpoints for the MAUI PDA (Move stock screen).</summary>
public static class PdaCatalogApiExtensions
{
    public static WebApplication MapPdaCatalogApi(this WebApplication app)
    {
        app.MapGet("/api/pda/catalog/warehouses", ListWarehousesAsync).RequireAuthorization("PdaOnly").DisableAntiforgery();
        app.MapGet("/api/pda/catalog/warehouses/{warehouseId:int}/locations", ListLocationsAsync).RequireAuthorization("PdaOnly").DisableAntiforgery();
        app.MapGet("/api/pda/catalog/items", ListItemsAsync).RequireAuthorization("PdaOnly").DisableAntiforgery();
        app.MapGet("/api/pda/catalog/summary", GetSummaryAsync).RequireAuthorization("PdaOnly").DisableAntiforgery();
        return app;
    }

    private static async Task<IResult> ListWarehousesAsync(
        [FromServices] IDbContextFactory<AppDbContext> dbFactory,
        CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var rows = await db.Warehouses.AsNoTracking()
            .Where(w => w.IsActive)
            .OrderBy(w => w.Code)
            .Select(w => new { w.Id, w.Code, w.Name })
            .ToListAsync(cancellationToken);
        return Results.Ok(rows);
    }

    private static async Task<IResult> ListLocationsAsync(
        int warehouseId,
        [FromServices] IDbContextFactory<AppDbContext> dbFactory,
        CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var exists = await db.Warehouses.AsNoTracking().AnyAsync(w => w.Id == warehouseId && w.IsActive, cancellationToken);
        if (!exists)
            return Results.NotFound("Warehouse not found or inactive.");

        var rows = await db.Locations.AsNoTracking()
            .Where(l => l.WarehouseId == warehouseId && l.IsActive)
            .OrderBy(l => l.Code)
            .Select(l => new { l.Id, l.Code, description = l.Description ?? "" })
            .ToListAsync(cancellationToken);
        return Results.Ok(rows);
    }

    /// <summary>Active items (catalog). PDA may filter client-side later (e.g. balances-only per README rules A–E).</summary>
    private static async Task<IResult> ListItemsAsync(
        [FromServices] IDbContextFactory<AppDbContext> dbFactory,
        CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var rows = await db.Items.AsNoTracking()
            .Where(i => i.IsActive)
            .OrderBy(i => i.Sku)
            .Select(i => new
            {
                i.Id,
                i.Sku,
                displayName = i.DisplayName,
                articleNumber = i.ArticleNumber ?? "",
                barcodes = i.Barcodes ?? ""
            })
            .ToListAsync(cancellationToken);
        return Results.Ok(rows);
    }

    private static async Task<IResult> GetSummaryAsync(
        int warehouseId,
        int locationId,
        int itemId,
        [FromServices] IDbContextFactory<AppDbContext> dbFactory,
        CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);

        var loc = await db.Locations.AsNoTracking()
            .Where(l => l.Id == locationId && l.WarehouseId == warehouseId && l.IsActive)
            .Select(l => new { l.Code, whCode = l.Warehouse!.Code, whName = l.Warehouse.Name })
            .FirstOrDefaultAsync(cancellationToken);
        if (loc is null)
            return Results.BadRequest("Invalid warehouse/location.");

        var item = await db.Items.AsNoTracking()
            .Where(i => i.Id == itemId && i.IsActive)
            .Select(i => new { i.Sku, i.DisplayName })
            .FirstOrDefaultAsync(cancellationToken);
        if (item is null)
            return Results.BadRequest("Invalid item.");

        var onHand = await db.StockBalances.AsNoTracking()
            .Where(b => b.WarehouseId == warehouseId && b.LocationId == locationId && b.ItemId == itemId)
            .Select(b => b.QuantityOnHand)
            .FirstOrDefaultAsync(cancellationToken);

        var mm = await db.MinMaxSettings.AsNoTracking()
            .Where(m => m.WarehouseId == warehouseId && m.LocationId == locationId && m.ItemId == itemId)
            .Select(m => new { m.Min, m.Max })
            .FirstOrDefaultAsync(cancellationToken);

        int? min = mm?.Min;
        int? max = mm?.Max;
        string status;
        if (min is null || max is null)
            status = "No min/max";
        else if (onHand < min.Value)
            status = "Below min";
        else if (onHand > max.Value)
            status = "Above max";
        else
            status = "Within range";

        return Results.Ok(new
        {
            warehouseCode = loc.whCode,
            warehouseName = loc.whName,
            locationCode = loc.Code,
            itemSku = item.Sku,
            itemName = item.DisplayName,
            onHand,
            min,
            max,
            status
        });
    }
}
