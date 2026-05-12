using Microsoft.EntityFrameworkCore;

namespace StockControl.Admin.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemBarcode> ItemBarcodes => Set<ItemBarcode>();
    public DbSet<MinMaxSetting> MinMaxSettings => Set<MinMaxSetting>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.Id);
            b.Property(x => x.Username).HasMaxLength(50).IsRequired();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<Warehouse>(b =>
        {
            b.ToTable("warehouses");
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).HasMaxLength(20).IsRequired();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Location>(b =>
        {
            b.ToTable("locations");
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).HasMaxLength(12).IsRequired();
            b.Property(x => x.Description).HasMaxLength(50).IsRequired(false);
            b.HasIndex(x => new { x.WarehouseId, x.Code }).IsUnique();
            b.HasOne(x => x.Warehouse).WithMany(x => x.Locations).HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Supplier>(b =>
        {
            b.ToTable("suppliers");
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).HasMaxLength(20).IsRequired();
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("products");
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).HasMaxLength(40).IsRequired();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
            b.HasOne(x => x.Supplier).WithMany(x => x.Products).HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Item>(b =>
        {
            b.ToTable("items");
            b.HasKey(x => x.Id);
            b.Property(x => x.Sku).HasMaxLength(40).IsRequired();
            b.Property(x => x.DisplayName).HasMaxLength(50).IsRequired();
            b.Property(x => x.Unit).HasMaxLength(10).IsRequired();
            b.Property(x => x.PackagingType).HasConversion<int>();
            b.Property(x => x.PackageQuantity).HasColumnType("decimal(18,3)").IsRequired();
            b.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();
            b.HasIndex(x => x.Sku).IsUnique();
            b.HasOne(x => x.Product).WithMany(x => x.Items).HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ItemBarcode>(b =>
        {
            b.ToTable("item_barcodes");
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).HasMaxLength(80).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
            b.HasOne(x => x.Item).WithMany(x => x.Barcodes).HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MinMaxSetting>(b =>
        {
            b.ToTable("minmax_settings");
            b.HasKey(x => x.Id);
            b.Property(x => x.Min).IsRequired();
            b.Property(x => x.Max).IsRequired();
            b.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.WarehouseId, x.ItemId, x.LocationId }).IsUnique();
            b.HasIndex(x => new { x.WarehouseId, x.ItemId })
                .IsUnique()
                .HasFilter("[LocationId] IS NULL");
        });

        modelBuilder.Entity<StockBalance>(b =>
        {
            b.ToTable("stock_balances");
            b.HasKey(x => x.Id);
            b.Property(x => x.QuantityOnHand).HasColumnType("decimal(18,3)").IsRequired();
            b.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.WarehouseId, x.LocationId, x.ItemId }).IsUnique();
        });
    }
}
