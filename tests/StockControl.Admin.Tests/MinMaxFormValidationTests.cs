using StockControl.Admin.Validation;
using Xunit;

namespace StockControl.Admin.Tests;

public class MinMaxFormValidationTests
{
    [Fact]
    public void Save_maps_warehouse_required_with_focus()
    {
        var issue = MinMaxSaveValidation.Validate(0, 1, 1, 0, 5);
        Assert.NotNull(issue);
        Assert.Equal("Warehouse is required.", issue.Value.Message);
        Assert.Equal("mm-edit-warehouse", issue.Value.FocusId);
    }

    [Fact]
    public void Save_maps_item_required_with_focus()
    {
        var issue = MinMaxSaveValidation.Validate(1, 0, 1, 0, 5);
        Assert.NotNull(issue);
        Assert.Equal("Item is required.", issue.Value.Message);
        Assert.Equal("mm-edit-item", issue.Value.FocusId);
    }

    [Fact]
    public void Save_maps_location_required_with_focus()
    {
        var issue = MinMaxSaveValidation.Validate(1, 1, 0, 0, 5);
        Assert.NotNull(issue);
        Assert.Equal("Location is required.", issue.Value.Message);
        Assert.Equal("mm-loc", issue.Value.FocusId);
    }

    [Fact]
    public void Save_rejects_negative_min() =>
        Assert.Equal("Min must be zero or greater.", MinMaxSaveValidation.Validate(1, 1, 1, -1, 5)!.Value.Message);

    [Fact]
    public void Save_rejects_negative_max() =>
        Assert.Equal("Max must be zero or greater.", MinMaxSaveValidation.Validate(1, 1, 1, 0, -1)!.Value.Message);

    [Fact]
    public void Save_rejects_max_less_than_min() =>
        Assert.Equal(
            "Max must be greater than or equal to Min.",
            MinMaxSaveValidation.Validate(1, 1, 1, 10, 5)!.Value.Message);

    [Fact]
    public void Save_accepts_equal_min_max() =>
        Assert.Null(MinMaxSaveValidation.Validate(1, 1, 1, 3, 3));

    [Fact]
    public void Save_accepts_valid_range() =>
        Assert.Null(MinMaxSaveValidation.Validate(1, 1, 1, 2, 10));
}
