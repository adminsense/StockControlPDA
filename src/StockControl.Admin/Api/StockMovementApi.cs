using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControl.Admin.Data;
using StockControl.Admin.Validation;

namespace StockControl.Admin.Api;

public sealed record StockMovementApiRequest(
    string LocationCode,
    string ItemCode,
    decimal Quantity,
    string Direction,
    int? UserId);

public static class StockMovementApiExtensions
{
    public static WebApplication MapStockMovementApi(this WebApplication app)
    {
        app.MapPost("/api/stock/movements", HandlePostAsync)
            .WithName("PostStockMovement")
            .DisableAntiforgery();
        return app;
    }

    private static async Task<IResult> HandlePostAsync(
        [FromBody] StockMovementApiRequest dto,
        [FromServices] IDbContextFactory<AppDbContext> dbFactory,
        CancellationToken cancellationToken)
    {
        var locationCode = (dto.LocationCode ?? "").Trim();
        if (locationCode.Length == 0)
            return Results.BadRequest("Location code is required.");
        if (locationCode.Length > LocationsSaveValidation.MaxCodeLength)
            return Results.BadRequest($"Location code must be {LocationsSaveValidation.MaxCodeLength} characters or fewer.");

        var itemCode = (dto.ItemCode ?? "").Trim();
        if (itemCode.Length == 0)
            return Results.BadRequest("Item code is required.");

        if (dto.Quantity <= 0m)
            return Results.BadRequest("Quantity must be greater than zero.");

        var direction = (dto.Direction ?? "").Trim().ToUpperInvariant();
        if (direction is not ("IN" or "OUT"))
            return Results.BadRequest("Direction must be IN or OUT.");

        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);

        var locationRows = await db.Locations
            .AsNoTracking()
            .Where(l => l.IsActive)
            .Select(l => new { l.Id, l.WarehouseId, l.Code })
            .ToListAsync(cancellationToken);

        var locations = locationRows
            .Where(l => string.Equals(l.Code, locationCode, StringComparison.OrdinalIgnoreCase))
            .Select(l => new { l.Id, l.WarehouseId })
            .ToList();

        if (locations.Count == 0)
            return Results.BadRequest("Unknown or inactive location.");
        if (locations.Count > 1)
            return Results.BadRequest("Ambiguous location code (exists in more than one warehouse).");

        var loc = locations[0];

        var skuUpper = itemCode.ToUpperInvariant();
        var itemId = await db.Items
            .AsNoTracking()
            .Where(i => i.IsActive && i.Sku.ToUpper() == skuUpper)
            .Select(i => i.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (itemId == 0)
        {
            var candidates = await db.Items
                .AsNoTracking()
                .Where(i => i.IsActive && i.Barcodes != null && i.Barcodes != "")
                .Select(i => new { i.Id, i.Barcodes })
                .ToListAsync(cancellationToken);

            foreach (var row in candidates)
            {
                if (ItemBarcodeInput.ParseLines(row.Barcodes).Contains(itemCode, StringComparer.OrdinalIgnoreCase))
                {
                    itemId = row.Id;
                    break;
                }
            }
        }

        if (itemId == 0)
        {
            itemId = await db.Items
                .AsNoTracking()
                .Where(i => i.IsActive && i.ArticleNumber.ToUpper() == itemCode.ToUpperInvariant())
                .Select(i => i.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (itemId == 0)
            return Results.BadRequest("Unknown or inactive item (SKU, barcode, or article number).");

        var userId = dto.UserId ?? 0;
        if (userId <= 0)
        {
            userId = await db.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .OrderBy(u => u.Id)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (userId <= 0)
            return Results.BadRequest("No active user in database (set userId in request for MVP testing).");

        var userExists = await db.Users.AsNoTracking().AnyAsync(u => u.Id == userId && u.IsActive, cancellationToken);
        if (!userExists)
            return Results.BadRequest("User is invalid or inactive.");

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var balance = await db.StockBalances
                .FirstOrDefaultAsync(
                    b => b.WarehouseId == loc.WarehouseId && b.LocationId == loc.Id && b.ItemId == itemId,
                    cancellationToken);

            var delta = direction == "IN" ? dto.Quantity : -dto.Quantity;

            if (balance is null)
            {
                if (delta < 0)
                {
                    await tx.RollbackAsync(cancellationToken);
                    return Results.BadRequest("Insufficient stock for outbound movement.");
                }

                balance = new StockBalance
                {
                    WarehouseId = loc.WarehouseId,
                    LocationId = loc.Id,
                    ItemId = itemId,
                    QuantityOnHand = delta,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                db.StockBalances.Add(balance);
            }
            else
            {
                var next = balance.QuantityOnHand + delta;
                if (next < 0)
                {
                    await tx.RollbackAsync(cancellationToken);
                    return Results.BadRequest("Insufficient stock for outbound movement.");
                }

                balance.QuantityOnHand = next;
                balance.UpdatedAt = DateTimeOffset.UtcNow;
            }

            db.StockMovements.Add(new StockMovement
            {
                WarehouseId = loc.WarehouseId,
                LocationId = loc.Id,
                ItemId = itemId,
                UserId = userId,
                Quantity = dto.Quantity,
                Direction = direction,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }

        return Results.Ok(new { ok = true, message = "Saved." });
    }
}
