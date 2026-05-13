namespace StockControl.Admin.Validation;

public static class WarehousesSaveValidation
{
    public const int MaxCodeLength = 20;
    public const int MaxNameLength = 50;

    public static string? ValidateSave(string codeTrimmedUpper, string nameTrimmed)
    {
        if (string.IsNullOrWhiteSpace(codeTrimmedUpper))
            return "Code is required.";
        if (codeTrimmedUpper.Length > MaxCodeLength)
            return "Code must be 20 characters or fewer.";
        if (string.IsNullOrWhiteSpace(nameTrimmed))
            return "Name is required.";
        if (nameTrimmed.Length > MaxNameLength)
            return "Name must be 50 characters or fewer.";
        return null;
    }
}
