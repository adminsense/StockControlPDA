namespace StockControl.Admin.Validation;

/// <summary>Client-side blur checks (matches previous Razor behavior: raw length, no trim).</summary>
public static class FieldBlurValidation
{
    public static string? MaxLengthExceeded(string fieldName, string? value, int maxLength)
    {
        var v = value ?? "";
        if (v.Length > maxLength)
            return $"{fieldName} must be {maxLength} characters or fewer.";
        return null;
    }
}
