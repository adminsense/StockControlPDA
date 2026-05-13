using StockControl.Admin.Validation;
using Xunit;

namespace StockControl.Admin.Tests;

public class ProductsFormValidationTests
{
    [Fact]
    public void Save_rejects_empty_code() =>
        Assert.Equal("Code is required.", ProductsSaveValidation.ValidateSave("", "Name", supplierId: 1));

    [Fact]
    public void Save_rejects_code_over_40() =>
        Assert.Equal(
            "Code must be 40 characters or fewer.",
            ProductsSaveValidation.ValidateSave(new string('P', 41), "Name", supplierId: 1));

    [Fact]
    public void Save_rejects_empty_name() =>
        Assert.Equal("Name is required.", ProductsSaveValidation.ValidateSave("PR1", "", supplierId: 1));

    [Fact]
    public void Save_rejects_name_over_100() =>
        Assert.Equal(
            "Name must be 100 characters or fewer.",
            ProductsSaveValidation.ValidateSave("PR1", new string('n', 101), supplierId: 1));

    [Fact]
    public void Save_rejects_missing_supplier() =>
        Assert.Equal("Supplier is required.", ProductsSaveValidation.ValidateSave("PR1", "Product", supplierId: 0));

    [Fact]
    public void Save_accepts_valid() =>
        Assert.Null(ProductsSaveValidation.ValidateSave("PR1", "Hammer", supplierId: 5));
}
