namespace StockControl.Admin.Validation;

public static class ItemBarcodeInput
{
    public const int MaxBarcodeLineLength = 80;

    public static List<string> ParseLines(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];
        return text
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string Join(IReadOnlyList<string> codes)
        => codes.Count == 0 ? "" : string.Join('\n', codes);

    public static string FormatForEditor(string? stored)
    {
        var lines = ParseLines(stored);
        return lines.Count == 0 ? "" : string.Join(Environment.NewLine, lines);
    }

    public static string? Preview(string? stored)
    {
        if (string.IsNullOrWhiteSpace(stored))
            return null;
        var lines = ParseLines(stored);
        return lines.Count == 0 ? null : string.Join(", ", lines);
    }

    public static string? ValidateLineLengths(IReadOnlyList<string> codes)
    {
        foreach (var code in codes)
        {
            if (code.Length > MaxBarcodeLineLength)
                return $"Each barcode must be 80 characters or fewer (\"{code}\" is too long).";
        }

        return null;
    }
}
