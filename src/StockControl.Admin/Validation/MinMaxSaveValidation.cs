namespace StockControl.Admin.Validation;

public readonly record struct MinMaxSaveIssue(string Message, string FocusId);

public static class MinMaxSaveValidation
{
    public static MinMaxSaveIssue? Validate(int warehouseId, int itemId, int locationId, int min, int max)
    {
        if (warehouseId <= 0)
            return new MinMaxSaveIssue("Warehouse is required.", "mm-edit-warehouse");
        if (itemId <= 0)
            return new MinMaxSaveIssue("Item is required.", "mm-edit-item");
        if (locationId <= 0)
            return new MinMaxSaveIssue("Location is required.", "mm-loc");
        if (min < 0)
            return new MinMaxSaveIssue("Min must be zero or greater.", "mm-min");
        if (max < 0)
            return new MinMaxSaveIssue("Max must be zero or greater.", "mm-max");
        if (max < min)
            return new MinMaxSaveIssue("Max must be greater than or equal to Min.", "mm-max");
        return null;
    }
}
