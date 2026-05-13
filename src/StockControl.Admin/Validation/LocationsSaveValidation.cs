namespace StockControl.Admin.Validation;

public static class LocationsSaveValidation
{
    public const int MaxCodeLength = 12;
    public const int MaxDescriptionLength = 50;

    public static string? ValidateSave(int warehouseId, string codeTrimmedUpper, string? descriptionOrNull)
    {
        if (warehouseId <= 0)
            return "Warehouse is required.";
        if (string.IsNullOrWhiteSpace(codeTrimmedUpper))
            return "Code is required.";
        if (codeTrimmedUpper.Length > MaxCodeLength)
            return "Code must be 12 characters or fewer.";
        if ((descriptionOrNull?.Length ?? 0) > MaxDescriptionLength)
            return "Description must be 50 characters or fewer.";
        return null;
    }
}
