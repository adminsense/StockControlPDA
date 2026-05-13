using StockControl.Admin.Validation;
using Xunit;

namespace StockControl.Admin.Tests;

public class UsersFormValidationTests
{
    [Fact]
    public void Save_rejects_empty_username()
    {
        Assert.Equal("Username is required.", UsersSaveValidation.ValidateSave("", "Name"));
        Assert.Equal("Username is required.", UsersSaveValidation.ValidateSave("   ", "Name"));
    }

    [Fact]
    public void Save_rejects_username_over_50_chars()
    {
        var u = new string('a', 51);
        Assert.Equal("Username must be 50 characters or fewer.", UsersSaveValidation.ValidateSave(u, "Ok"));
    }

    [Fact]
    public void Save_accepts_username_at_max_length()
    {
        var u = new string('a', 50);
        Assert.Null(UsersSaveValidation.ValidateSave(u, "Name"));
    }

    [Fact]
    public void Save_rejects_empty_name()
    {
        Assert.Equal("Name is required.", UsersSaveValidation.ValidateSave("user", ""));
        Assert.Equal("Name is required.", UsersSaveValidation.ValidateSave("user", "  "));
    }

    [Fact]
    public void Save_rejects_name_over_50_chars()
    {
        var n = new string('b', 51);
        Assert.Equal("Name must be 50 characters or fewer.", UsersSaveValidation.ValidateSave("user", n));
    }

    [Theory]
    [InlineData("u", "n")]
    [InlineData("alice", "Alice Smith")]
    public void Save_accepts_valid_pairs(string u, string n) =>
        Assert.Null(UsersSaveValidation.ValidateSave(u, n));

    [Fact]
    public void Blur_uses_raw_length_while_save_validation_uses_trimmed_values()
    {
        var raw = "  " + new string('x', 49);
        Assert.Equal(51, raw.Length);
        Assert.Equal(49, raw.Trim().Length);
        Assert.Null(FieldBlurValidation.MaxLengthExceeded("Username", raw.Trim(), UsersSaveValidation.MaxUsernameLength));
        Assert.Equal(
            "Username must be 50 characters or fewer.",
            FieldBlurValidation.MaxLengthExceeded("Username", raw, UsersSaveValidation.MaxUsernameLength));
        Assert.Null(UsersSaveValidation.ValidateSave(raw.Trim(), "Name"));
    }
}
