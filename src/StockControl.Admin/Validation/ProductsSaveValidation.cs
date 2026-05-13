namespace StockControl.Admin.Validation;

public static class ProductsSaveValidation
{
    public const int MaxCodeLength = 40;
    public const int MaxNameLength = 100;

    public static string? ValidateSave(string codeTrimmedUpper, string nameTrimmed, int supplierId)
    {
        if (string.IsNullOrWhiteSpace(codeTrimmedUpper))
            return "Code is required.";
        if (codeTrimmedUpper.Length > MaxCodeLength)
            return "Code must be 40 characters or fewer.";
        if (string.IsNullOrWhiteSpace(nameTrimmed))
            return "Name is required.";
        if (nameTrimmed.Length > MaxNameLength)
            return "Name must be 100 characters or fewer.";
        if (supplierId <= 0)
            return "Supplier is required.";
        return null;
    }
}
