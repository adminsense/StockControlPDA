-- StockControl.Admin seed data: Items + Barcodes (idempotent)
-- Target DB: StockControl (SQL Server)
--
-- Safe to run multiple times: inserts only if missing.

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

    -- Items
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

    -- Item barcodes (unique on Code)
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

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH

