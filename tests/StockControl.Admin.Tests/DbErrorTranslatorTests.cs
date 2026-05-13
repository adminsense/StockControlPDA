using Microsoft.EntityFrameworkCore;
using StockControl.Admin.Data;
using Xunit;

namespace StockControl.Admin.Tests;

public class DbErrorTranslatorTests
{
    [Fact]
    public void DbUpdateException_maps_to_constraint_message()
    {
        var ex = new DbUpdateException("fail", (Exception?)null);
        Assert.True(DbErrorTranslator.TryGetUserFriendlyMessage(ex, out var msg));
        Assert.Contains("constraint", msg, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Unrelated_exception_returns_false()
    {
        Assert.False(DbErrorTranslator.TryGetUserFriendlyMessage(new InvalidOperationException("x"), out _));
    }
}
