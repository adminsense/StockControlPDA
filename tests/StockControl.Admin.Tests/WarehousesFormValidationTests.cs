using StockControl.Admin.Validation;
using Xunit;

namespace StockControl.Admin.Tests;

public class WarehousesFormValidationTests
{
    [Fact]
    public void Save_rejects_empty_code() =>
        Assert.Equal("Code is required.", WarehousesSaveValidation.ValidateSave("", "Name"));

    [Fact]
    public void Save_rejects_code_over_20() =>
        Assert.Equal(
            "Code must be 20 characters or fewer.",
            WarehousesSaveValidation.ValidateSave(new string('A', 21), "Name"));

    [Fact]
    public void Save_rejects_empty_name() =>
        Assert.Equal("Name is required.", WarehousesSaveValidation.ValidateSave("WH1", ""));

    [Fact]
    public void Save_rejects_name_over_50() =>
        Assert.Equal(
            "Name must be 50 characters or fewer.",
            WarehousesSaveValidation.ValidateSave("WH1", new string('n', 51)));

    [Fact]
    public void Save_accepts_valid() =>
        Assert.Null(WarehousesSaveValidation.ValidateSave("WH1", "Main warehouse"));

    [Fact]
    public void Blur_uses_raw_length()
    {
        var code = new string('A', 21);
        Assert.Equal(
            "Code must be 20 characters or fewer.",
            FieldBlurValidation.MaxLengthExceeded("Code", code, WarehousesSaveValidation.MaxCodeLength));
    }
}
