namespace StockControl.Admin.Validation;

public static class SuppliersSaveValidation
{
    public const int MaxCodeLength = 20;
    public const int MaxNameLength = 100;

    public static string? ValidateSave(string codeTrimmedUpper, string nameTrimmed)
    {
        if (string.IsNullOrWhiteSpace(codeTrimmedUpper))
            return "Code is required.";
        if (codeTrimmedUpper.Length > MaxCodeLength)
            return "Code must be 20 characters or fewer.";
        if (string.IsNullOrWhiteSpace(nameTrimmed))
            return "Name is required.";
        if (nameTrimmed.Length > MaxNameLength)
            return "Name must be 100 characters or fewer.";
        return null;
    }
}
