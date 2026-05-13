using StockControl.Admin.Validation;
using Xunit;

namespace StockControl.Admin.Tests;

public class SuppliersFormValidationTests
{
    [Fact]
    public void Save_rejects_empty_code() =>
        Assert.Equal("Code is required.", SuppliersSaveValidation.ValidateSave("", "Acme"));

    [Fact]
    public void Save_rejects_code_over_20() =>
        Assert.Equal(
            "Code must be 20 characters or fewer.",
            SuppliersSaveValidation.ValidateSave(new string('S', 21), "Acme"));

    [Fact]
    public void Save_rejects_empty_name() =>
        Assert.Equal("Name is required.", SuppliersSaveValidation.ValidateSave("ACME", ""));

    [Fact]
    public void Save_rejects_name_over_100() =>
        Assert.Equal(
            "Name must be 100 characters or fewer.",
            SuppliersSaveValidation.ValidateSave("ACME", new string('n', 101)));

    [Fact]
    public void Save_accepts_valid() =>
        Assert.Null(SuppliersSaveValidation.ValidateSave("ACME", "Acme Supplies"));
}
