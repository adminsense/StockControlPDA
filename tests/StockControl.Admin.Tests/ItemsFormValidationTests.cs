using StockControl.Admin.Data;
using StockControl.Admin.Validation;
using Xunit;

namespace StockControl.Admin.Tests;

public class ItemsFormValidationTests
{
    private static string? Validate(
        string sku,
        string article,
        string display,
        string unit,
        int productId,
        decimal pkgQty,
        PackagingType packaging,
        decimal price,
        string barcodesText) =>
        ItemsSaveValidation.ValidateSave(
            sku,
            article,
            display,
            unit,
            productId,
            pkgQty,
            packaging,
            price,
            ItemBarcodeInput.ParseLines(barcodesText));

    [Fact]
    public void Save_rejects_empty_sku() =>
        Assert.Equal("SKU is required.", Validate("", "a", "n", "ST", 1, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_sku_over_40() =>
        Assert.Equal(
            "SKU must be 40 characters or fewer.",
            Validate(new string('S', 41), "a", "n", "ST", 1, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_article_over_50() =>
        Assert.Equal(
            "Article number must be 50 characters or fewer.",
            Validate("SKU", new string('a', 51), "n", "ST", 1, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_empty_display_name() =>
        Assert.Equal("Display name is required.", Validate("SKU", "a", "", "ST", 1, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_display_name_over_100() =>
        Assert.Equal(
            "Display name must be 100 characters or fewer.",
            Validate("SKU", "a", new string('d', 101), "ST", 1, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_empty_unit() =>
        Assert.Equal("Quantity unit is required.", Validate("SKU", "a", "Name", "", 1, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_unit_over_10() =>
        Assert.Equal(
            "Quantity unit must be 10 characters or fewer.",
            Validate("SKU", "a", "Name", new string('U', 11), 1, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_missing_product() =>
        Assert.Equal("Product is required.", Validate("SKU", "a", "Name", "ST", 0, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_non_positive_package_quantity() =>
        Assert.Equal(
            "Package quantity must be greater than zero.",
            Validate("SKU", "a", "Name", "ST", 1, 0, PackagingType.Box, 0, ""));

    [Fact]
    public void Save_rejects_unit_packaging_with_quantity_not_one() =>
        Assert.Equal(
            "When packaging is Unit (each piece), package quantity must be 1.",
            Validate("SKU", "a", "Name", "ST", 1, 2, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_accepts_unit_packaging_with_quantity_one() =>
        Assert.Null(Validate("SKU", "a", "Name", "ST", 1, 1, PackagingType.Unit, 0, ""));

    [Fact]
    public void Save_rejects_negative_price() =>
        Assert.Equal("Price must be zero or greater.", Validate("SKU", "a", "Name", "ST", 1, 1, PackagingType.Unit, -0.01m, ""));

    [Fact]
    public void Save_rejects_barcode_line_over_80_chars()
    {
        var longCode = new string('1', 81);
        var err = Validate("SKU", "a", "Name", "ST", 1, 1, PackagingType.Unit, 0, longCode);
        Assert.Equal($"Each barcode must be 80 characters or fewer (\"{longCode}\" is too long).", err);
    }

    [Fact]
    public void ParseLines_trims_splits_deduplicates_case_insensitively()
    {
        var lines = ItemBarcodeInput.ParseLines(" b \nB \n a \r\n");
        Assert.Equal(new[] { "a", "b" }, lines);
    }

    [Fact]
    public void Join_round_trips_with_parse()
    {
        var codes = ItemBarcodeInput.ParseLines("z\ny");
        Assert.Equal("y\nz", ItemBarcodeInput.Join(codes));
    }

    [Fact]
    public void FormatForEditor_uses_system_newline_between_lines()
    {
        var text = ItemBarcodeInput.FormatForEditor("x\ny");
        Assert.Equal($"x{Environment.NewLine}y", text);
    }

    [Fact]
    public void Preview_joins_with_comma_space()
    {
        Assert.Equal("a, b", ItemBarcodeInput.Preview("b\na"));
    }

    [Fact]
    public void Blur_max_length_matches_items_fields()
    {
        Assert.Equal(
            "SKU must be 40 characters or fewer.",
            FieldBlurValidation.MaxLengthExceeded("SKU", new string('x', 41), ItemsSaveValidation.MaxSkuLength));
    }
}
