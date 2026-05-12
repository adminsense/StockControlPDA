namespace StockControl.Admin.Data;

public abstract class EntityBase
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}

public enum PackagingType
{
    Unit = 0,
    Box = 1,
    Case = 2,
    Pallet = 3,
    Bag = 4,
    Kit = 5
}

public sealed class AppUser : EntityBase
{
    public string Username { get; set; } = "";
    public string Name { get; set; } = "";
}

public sealed class Warehouse : EntityBase
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";

    public List<Location> Locations { get; set; } = [];
}

public sealed class Location : EntityBase
{
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public string Code { get; set; } = "";
    public string? Description { get; set; }
}

public sealed class Supplier : EntityBase
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";

    public List<Product> Products { get; set; } = [];
}

public sealed class Product : EntityBase
{
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public string Code { get; set; } = "";
    public string Name { get; set; } = "";

    public List<Item> Items { get; set; } = [];
}

public sealed class Item : EntityBase
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public string Sku { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Unit { get; set; } = "";

    public PackagingType PackagingType { get; set; } = PackagingType.Unit;
    public decimal PackageQuantity { get; set; } = 1m;
    public decimal Price { get; set; }

    public List<ItemBarcode> Barcodes { get; set; } = [];
}

public sealed class ItemBarcode : EntityBase
{
    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public string Code { get; set; } = "";
}

public sealed class MinMaxSetting : EntityBase
{
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public int Min { get; set; }
    public int Max { get; set; }
}

public sealed class StockBalance : EntityBase
{
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public decimal QuantityOnHand { get; set; }
}
