using StockControl.Admin.Data;

namespace StockControl.Admin.Validation;

public static class ItemsSaveValidation
{
    public const int MaxSkuLength = 40;
    public const int MaxArticleLength = 100;
    public const int MaxDisplayNameLength = 200;
    public const int MaxUnitLength = 10;

    public static string? ValidateSave(
        string skuTrimmedUpper,
        string articleTrimmed,
        string displayNameTrimmed,
        string unitTrimmedUpper,
        int productId,
        decimal packageQuantity,
        PackagingType packagingType,
        decimal price,
        IReadOnlyList<string> barcodeLines)
    {
        if (string.IsNullOrWhiteSpace(skuTrimmedUpper))
            return "SKU is required.";
        if (skuTrimmedUpper.Length > MaxSkuLength)
            return "SKU must be 40 characters or fewer.";
        if (articleTrimmed.Length > MaxArticleLength)
            return "Article number must be 100 characters or fewer.";
        if (string.IsNullOrWhiteSpace(displayNameTrimmed))
            return "Display name is required.";
        if (displayNameTrimmed.Length > MaxDisplayNameLength)
            return "Display name must be 200 characters or fewer.";
        if (string.IsNullOrWhiteSpace(unitTrimmedUpper))
            return "Quantity unit is required.";
        if (unitTrimmedUpper.Length > MaxUnitLength)
            return "Quantity unit must be 10 characters or fewer.";
        if (productId <= 0)
            return "Product is required.";
        if (packageQuantity <= 0m)
            return "Package quantity must be greater than zero.";
        if (packagingType == PackagingType.Unit && packageQuantity != 1m)
            return "When packaging is Unit (each piece), package quantity must be 1.";
        if (price < 0m)
            return "Price must be zero or greater.";
        return ItemBarcodeInput.ValidateLineLengths(barcodeLines);
    }
}
