using StockControl.Admin.Validation;
using Xunit;

namespace StockControl.Admin.Tests;

public class StockPageTests
{
    private static StockFilterRow Row(
        int wh = 1,
        int loc = 10,
        string sku = "S1",
        string article = "A1",
        string product = "P1",
        string name = "N1",
        decimal qty = 5,
        int? min = null,
        int? max = null) =>
        new(wh, loc, product, sku, article, name, qty, min, max);

    [Fact]
    public void Filter_matches_warehouse()
    {
        var r = Row(wh: 2);
        Assert.True(StockGridFilter.Matches(r, 2, 0, "", ""));
        Assert.False(StockGridFilter.Matches(r, 1, 0, "", ""));
    }

    [Fact]
    public void Filter_matches_location()
    {
        var r = Row(loc: 99);
        Assert.True(StockGridFilter.Matches(r, 0, 99, "", ""));
        Assert.False(StockGridFilter.Matches(r, 0, 10, "", ""));
    }

    [Fact]
    public void Filter_search_matches_sku_case_insensitive()
    {
        var r = Row(sku: "AbC-12");
        Assert.True(StockGridFilter.Matches(r, 0, 0, "abc", ""));
        Assert.False(StockGridFilter.Matches(r, 0, 0, "xyz", ""));
    }

    [Fact]
    public void Filter_search_matches_article_when_present()
    {
        var r = Row(article: "ART-9");
        Assert.True(StockGridFilter.Matches(r, 0, 0, "art", ""));
    }

    [Fact]
    public void Filter_search_ignores_empty_article()
    {
        var r = Row(article: "");
        Assert.False(StockGridFilter.Matches(r, 0, 0, "only-in-article", ""));
        Assert.True(StockGridFilter.Matches(r, 0, 0, "S1", ""));
    }

    [Fact]
    public void Filter_below_requires_min_and_qty()
    {
        var ok = Row(qty: 5, min: 3, max: 10);
        var below = Row(qty: 2, min: 3, max: 10);
        Assert.False(StockGridFilter.Matches(ok, 0, 0, "", "below"));
        Assert.True(StockGridFilter.Matches(below, 0, 0, "", "below"));
    }

    [Fact]
    public void Filter_above_requires_max_and_qty()
    {
        var ok = Row(qty: 5, min: 1, max: 10);
        var above = Row(qty: 11, min: 1, max: 10);
        Assert.False(StockGridFilter.Matches(ok, 0, 0, "", "above"));
        Assert.True(StockGridFilter.Matches(above, 0, 0, "", "above"));
    }

    [Fact]
    public void Paging_max_page_index()
    {
        Assert.Equal(0, StockGridPaging.MaxPageIndex(0, 10));
        Assert.Equal(0, StockGridPaging.MaxPageIndex(10, 10));
        Assert.Equal(1, StockGridPaging.MaxPageIndex(11, 10));
    }

    [Fact]
    public void Paging_visible_range()
    {
        Assert.Equal(0, StockGridPaging.ShowFrom(0, 0, 10));
        Assert.Equal(1, StockGridPaging.ShowFrom(5, 0, 10));
        Assert.Equal(11, StockGridPaging.ShowFrom(15, 1, 10));
        Assert.Equal(5, StockGridPaging.ShowTo(5, 0, 10));
        Assert.Equal(10, StockGridPaging.ShowTo(15, 0, 10));
        Assert.Equal(15, StockGridPaging.ShowTo(15, 1, 10));
    }
}
