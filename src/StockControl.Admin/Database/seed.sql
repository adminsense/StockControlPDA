-- StockControl.Admin seed data (ALL tables) (idempotent)
-- Target DB: StockControl (SQL Server)
--
-- This script inserts a small dataset for testing the Admin UI.
-- It is safe to run multiple times: it only inserts rows if they do not exist.

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

BEGIN TRY
    BEGIN TRAN;

    DECLARE @now datetimeoffset(7) = SYSDATETIMEOFFSET();

    -- Warehouses
    IF NOT EXISTS (SELECT 1 FROM dbo.warehouses WHERE Code = N'WH01')
        INSERT INTO dbo.warehouses (Code, Name, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'WH01', N'Main', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.warehouses WHERE Code = N'WH02')
        INSERT INTO dbo.warehouses (Code, Name, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'WH02', N'Branch', 1, @now, NULL);

    DECLARE @wh1 int = (SELECT TOP (1) Id FROM dbo.warehouses WHERE Code = N'WH01');
    DECLARE @wh2 int = (SELECT TOP (1) Id FROM dbo.warehouses WHERE Code = N'WH02');

    -- Locations
    IF NOT EXISTS (SELECT 1 FROM dbo.locations WHERE WarehouseId = @wh1 AND Code = N'A01-01')
        INSERT INTO dbo.locations (WarehouseId, Code, Description, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh1, N'A01-01', N'Aisle A', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.locations WHERE WarehouseId = @wh1 AND Code = N'B01-01')
        INSERT INTO dbo.locations (WarehouseId, Code, Description, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh1, N'B01-01', N'Aisle B', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.locations WHERE WarehouseId = @wh2 AND Code = N'C01-01')
        INSERT INTO dbo.locations (WarehouseId, Code, Description, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh2, N'C01-01', N'Front', 1, @now, NULL);

    DECLARE @locA int = (SELECT TOP (1) Id FROM dbo.locations WHERE WarehouseId = @wh1 AND Code = N'A01-01');
    DECLARE @locB int = (SELECT TOP (1) Id FROM dbo.locations WHERE WarehouseId = @wh1 AND Code = N'B01-01');
    DECLARE @locC int = (SELECT TOP (1) Id FROM dbo.locations WHERE WarehouseId = @wh2 AND Code = N'C01-01');

    -- Users
    IF NOT EXISTS (SELECT 1 FROM dbo.users WHERE Username = N'marco')
        INSERT INTO dbo.users (Username, Name, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'marco', N'Marco', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.users WHERE Username = N'operador01')
        INSERT INTO dbo.users (Username, Name, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'operador01', N'Operador 01', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.users WHERE Username = N'teste')
        INSERT INTO dbo.users (Username, Name, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'teste', N'Teste', 0, @now, NULL);

    -- Products (optional master-data)
    IF NOT EXISTS (SELECT 1 FROM dbo.products WHERE Code = N'PRD-0001')
        INSERT INTO dbo.products (Code, Name, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'PRD-0001', N'Fasteners', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.products WHERE Code = N'PRD-0002')
        INSERT INTO dbo.products (Code, Name, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'PRD-0002', N'Cable ties', 1, @now, NULL);

    -- Items (a few to test UI)
    IF NOT EXISTS (SELECT 1 FROM dbo.items WHERE Sku = N'SKU-0001')
        INSERT INTO dbo.items (Sku, DisplayName, Unit, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'SKU-0001', N'Fasteners M6', N'UN', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.items WHERE Sku = N'SKU-0002')
        INSERT INTO dbo.items (Sku, DisplayName, Unit, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'SKU-0002', N'Cable tie 200mm', N'CX', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.items WHERE Sku = N'SKU-0003')
        INSERT INTO dbo.items (Sku, DisplayName, Unit, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'SKU-0003', N'Safety gloves (M)', N'PR', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.items WHERE Sku = N'SKU-0004')
        INSERT INTO dbo.items (Sku, DisplayName, Unit, IsActive, CreatedAt, UpdatedAt)
        VALUES (N'SKU-0004', N'Electrical tape', N'RL', 1, @now, NULL);

    DECLARE @it1 int = (SELECT TOP (1) Id FROM dbo.items WHERE Sku = N'SKU-0001');
    DECLARE @it2 int = (SELECT TOP (1) Id FROM dbo.items WHERE Sku = N'SKU-0002');
    DECLARE @it3 int = (SELECT TOP (1) Id FROM dbo.items WHERE Sku = N'SKU-0003');
    DECLARE @it4 int = (SELECT TOP (1) Id FROM dbo.items WHERE Sku = N'SKU-0004');

    -- Item barcodes
    IF NOT EXISTS (SELECT 1 FROM dbo.item_barcodes WHERE Code = N'8712345678901')
        INSERT INTO dbo.item_barcodes (ItemId, Code, IsActive, CreatedAt, UpdatedAt)
        VALUES (@it1, N'8712345678901', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.item_barcodes WHERE Code = N'8712345678902')
        INSERT INTO dbo.item_barcodes (ItemId, Code, IsActive, CreatedAt, UpdatedAt)
        VALUES (@it2, N'8712345678902', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.item_barcodes WHERE Code = N'8712345678903')
        INSERT INTO dbo.item_barcodes (ItemId, Code, IsActive, CreatedAt, UpdatedAt)
        VALUES (@it3, N'8712345678903', 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.item_barcodes WHERE Code = N'8712345678904')
        INSERT INTO dbo.item_barcodes (ItemId, Code, IsActive, CreatedAt, UpdatedAt)
        VALUES (@it4, N'8712345678904', 1, @now, NULL);

    -- Min/Max settings
    IF NOT EXISTS (SELECT 1 FROM dbo.minmax_settings WHERE WarehouseId = @wh1 AND ItemId = @it1 AND LocationId IS NULL)
        INSERT INTO dbo.minmax_settings (WarehouseId, ItemId, LocationId, Min, Max, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh1, @it1, NULL, 10, 200, 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.minmax_settings WHERE WarehouseId = @wh1 AND ItemId = @it2 AND LocationId = @locA)
        INSERT INTO dbo.minmax_settings (WarehouseId, ItemId, LocationId, Min, Max, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh1, @it2, @locA, 40, 120, 1, @now, NULL);

    -- Stock balances (the only stock table)
    IF NOT EXISTS (SELECT 1 FROM dbo.stock_balances WHERE WarehouseId = @wh1 AND LocationId = @locA AND ItemId = @it1)
        INSERT INTO dbo.stock_balances (WarehouseId, LocationId, ItemId, QuantityOnHand, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh1, @locA, @it1, 8.000, 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.stock_balances WHERE WarehouseId = @wh1 AND LocationId = @locA AND ItemId = @it2)
        INSERT INTO dbo.stock_balances (WarehouseId, LocationId, ItemId, QuantityOnHand, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh1, @locA, @it2, 65.000, 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.stock_balances WHERE WarehouseId = @wh1 AND LocationId = @locB AND ItemId = @it1)
        INSERT INTO dbo.stock_balances (WarehouseId, LocationId, ItemId, QuantityOnHand, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh1, @locB, @it1, 140.000, 1, @now, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.stock_balances WHERE WarehouseId = @wh2 AND LocationId = @locC AND ItemId = @it2)
        INSERT INTO dbo.stock_balances (WarehouseId, LocationId, ItemId, QuantityOnHand, IsActive, CreatedAt, UpdatedAt)
        VALUES (@wh2, @locC, @it2, 10.000, 1, @now, NULL);

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH

