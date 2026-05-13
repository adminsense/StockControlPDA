using StockControl.Admin.Validation;
using Xunit;

namespace StockControl.Admin.Tests;

public class LocationsFormValidationTests
{
    [Fact]
    public void Save_rejects_missing_warehouse() =>
        Assert.Equal("Warehouse is required.", LocationsSaveValidation.ValidateSave(0, "A1", null));

    [Fact]
    public void Save_rejects_empty_code() =>
        Assert.Equal("Code is required.", LocationsSaveValidation.ValidateSave(1, "", null));

    [Fact]
    public void Save_rejects_code_over_12() =>
        Assert.Equal(
            "Code must be 12 characters or fewer.",
            LocationsSaveValidation.ValidateSave(1, new string('L', 13), null));

    [Fact]
    public void Save_rejects_description_over_50() =>
        Assert.Equal(
            "Description must be 50 characters or fewer.",
            LocationsSaveValidation.ValidateSave(1, "A1", new string('d', 51)));

    [Fact]
    public void Save_accepts_null_description() =>
        Assert.Null(LocationsSaveValidation.ValidateSave(1, "BIN-01", null));

    [Fact]
    public void Save_accepts_description_at_boundary() =>
        Assert.Null(LocationsSaveValidation.ValidateSave(1, "BIN-01", new string('x', 50)));
}
