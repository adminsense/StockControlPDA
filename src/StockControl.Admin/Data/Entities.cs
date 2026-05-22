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

/// <summary>Stored as int in <c>users.Role</c>: 1 = admin (web), 2 = admin_pda (PDA).</summary>
public enum UserRole
{
    Admin = 1,
    AdminPda = 2
}

public sealed class AppUser : EntityBase
{
    public string Username { get; set; } = "";
    public string Name { get; set; } = "";
    public UserRole Role { get; set; } = UserRole.AdminPda;
    /// <summary>Hashed password; column <c>Password</c> in SQL.</summary>
    public string? PasswordHash { get; set; }
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
    /// <summary>Supplier / catalog article number (Artikelnummer); distinct from internal SKU.</summary>
    public string ArticleNumber { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Unit { get; set; } = "";

    public PackagingType PackagingType { get; set; } = PackagingType.Unit;
    public decimal PackageQuantity { get; set; } = 1m;
    public decimal Price { get; set; }

    /// <summary>Scanner / wedge codes, one per line (newline-separated). Unique across all items (enforced in app on save).</summary>
    public string Barcodes { get; set; } = "";
}

public sealed class MinMaxSetting : EntityBase
{
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public int LocationId { get; set; }
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

/// <summary>Immutable stock movement line (PDA / API). Quantity is always positive; direction is IN or OUT.</summary>
public sealed class StockMovement
{
    public long Id { get; set; }

    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public int UserId { get; set; }
    public AppUser? User { get; set; }

    public decimal Quantity { get; set; }

    /// <summary>IN or OUT (stored uppercase).</summary>
    public string Direction { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>Immutable audit trail row (see readme/audit_stock.md).</summary>
public sealed class AuditLog
{
    public long Id { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string Action { get; set; } = "";
    public string EntityName { get; set; } = "";
    public int? EntityId { get; set; }
    public int? UserId { get; set; }
    public string Username { get; set; } = "";
    public bool Success { get; set; } = true;
    public string Severity { get; set; } = "Information";
    public string? IpAddress { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ErrorMessage { get; set; }
}
