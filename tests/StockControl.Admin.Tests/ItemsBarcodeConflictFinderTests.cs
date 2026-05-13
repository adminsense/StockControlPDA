using Microsoft.EntityFrameworkCore;
using StockControl.Admin.Data;
using Xunit;

namespace StockControl.Admin.Tests;

public class ItemsBarcodeConflictFinderTests
{
    [Fact]
    public async Task Returns_null_when_no_incoming_codes()
    {
        await using var db = CreateDb();
        var conflict = await ItemBarcodeConflictFinder.FindConflictAsync(db, editingItemId: 1, []);
        Assert.Null(conflict);
    }

    [Fact]
    public async Task Detects_conflict_with_existing_item_barcodes()
    {
        await using var db = CreateDb();
        SeedMinimalCatalog(db, otherBarcodes: "SHARED\nOTHER");

        var conflict = await ItemBarcodeConflictFinder.FindConflictAsync(
            db,
            editingItemId: 2,
            ["shared"]);
        Assert.Equal("Barcode \"SHARED\" is already used on another item.", conflict);
    }

    [Fact]
    public async Task Same_item_incoming_codes_do_not_conflict_with_self()
    {
        await using var db = CreateDb();
        var supplier = new Supplier { Code = "S", Name = "Supplier" };
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();

        var product = new Product { SupplierId = supplier.Id, Code = "P", Name = "Product" };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var item = new Item
        {
            ProductId = product.Id,
            Sku = "SKU-1",
            ArticleNumber = "A1",
            DisplayName = "One",
            Unit = "ST",
            Barcodes = "CODE-A\nCODE-B"
        };
        db.Items.Add(item);
        await db.SaveChangesAsync();

        var conflict = await ItemBarcodeConflictFinder.FindConflictAsync(
            db,
            editingItemId: item.Id,
            ["code-a", "code-b"]);
        Assert.Null(conflict);
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    /// <summary>Creates supplier, product, two items; returns the second item (editing target).</summary>
    private static Item SeedMinimalCatalog(AppDbContext db, string otherBarcodes)
    {
        var supplier = new Supplier { Code = "S", Name = "Supplier" };
        db.Suppliers.Add(supplier);
        db.SaveChanges();

        var product = new Product { SupplierId = supplier.Id, Code = "P", Name = "Product" };
        db.Products.Add(product);
        db.SaveChanges();

        var item1 = new Item
        {
            ProductId = product.Id,
            Sku = "SKU-1",
            ArticleNumber = "A1",
            DisplayName = "One",
            Unit = "ST",
            Barcodes = otherBarcodes
        };
        var item2 = new Item
        {
            ProductId = product.Id,
            Sku = "SKU-2",
            ArticleNumber = "A2",
            DisplayName = "Two",
            Unit = "ST",
            Barcodes = ""
        };
        db.Items.AddRange(item1, item2);
        db.SaveChanges();
        return item2;
    }
}
