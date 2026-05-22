using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using StockControl.PDA.Services;

namespace StockControl.PDA;

public partial class MainPage : ContentPage
{
    /// <summary>Location codes are validated up to 12 characters (Admin + PDA readme).</summary>
    private const int MaxLocationCodeChars = 12;

    private readonly IStockMovementClient _stockClient =
        ServiceHelper.Services.GetRequiredService<IStockMovementClient>();

    private readonly ICatalogSyncClient _catalogSync =
        ServiceHelper.Services.GetRequiredService<ICatalogSyncClient>();

    private readonly IMoveStockCatalogClient _catalog =
        ServiceHelper.Services.GetRequiredService<IMoveStockCatalogClient>();

    private List<CatalogWarehouse> _warehouses = [];
    private List<CatalogLocation> _locations = [];
    private List<CatalogItem> _items = [];

    private int _qty = 1;
    private bool _suppressPickerEvents;
    private CancellationTokenSource? _scanMessageCts;

    public MainPage()
    {
        InitializeComponent();
        ClearSummaryLabels();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var session = ServiceHelper.Services.GetRequiredService<IAuthSession>();
        if (!await session.IsLoggedInAsync())
        {
            await Shell.Current.GoToAsync("//Login");
            return;
        }

        var name = await session.GetDisplayNameAsync();
        UserLabel.Text = string.IsNullOrWhiteSpace(name) ? "" : $"Signed in as {name}";

        await LoadInitialCatalogAsync();
        Dispatcher.Dispatch(() => ScanEntry.Focus());
    }

    private async void OnSignOutClicked(object? sender, EventArgs e)
    {
        var session = ServiceHelper.Services.GetRequiredService<IAuthSession>();
        await session.ClearAsync();
        await Shell.Current.GoToAsync("//Login");
    }

    private async Task LoadInitialCatalogAsync()
    {
        var warehouses = await _catalog.GetWarehousesAsync();
        var items = await _catalog.GetItemsAsync();
        _warehouses = warehouses.ToList();
        _items = items.ToList();

        _suppressPickerEvents = true;
        try
        {
            WarehousePicker.ItemsSource = _warehouses;
            WarehousePicker.SelectedIndex = _warehouses.Count > 0 ? 0 : -1;

            ItemPicker.ItemsSource = _items;
            ItemPicker.SelectedIndex = _items.Count > 0 ? 0 : -1;
        }
        finally
        {
            _suppressPickerEvents = false;
        }

        await LoadLocationsForSelectedWarehouseAsync();
        await RefreshSummaryAsync();
    }

    private async Task ReloadCatalogAfterSyncAsync()
    {
        var warehouses = await _catalog.GetWarehousesAsync();
        var items = await _catalog.GetItemsAsync();
        _warehouses = warehouses.ToList();
        _items = items.ToList();

        _suppressPickerEvents = true;
        try
        {
            WarehousePicker.ItemsSource = _warehouses;
            WarehousePicker.SelectedIndex = _warehouses.Count > 0 ? 0 : -1;

            ItemPicker.ItemsSource = _items;
            ItemPicker.SelectedIndex = _items.Count > 0 ? 0 : -1;
        }
        finally
        {
            _suppressPickerEvents = false;
        }

        await LoadLocationsForSelectedWarehouseAsync();
        await RefreshSummaryAsync();
    }

    private async void OnWarehousePickerChanged(object? sender, EventArgs e)
    {
        if (_suppressPickerEvents)
            return;

        await LoadLocationsForSelectedWarehouseAsync();
        await RefreshSummaryAsync();
    }

    private async void OnLocationOrItemChanged(object? sender, EventArgs e)
    {
        if (_suppressPickerEvents)
            return;

        await RefreshSummaryAsync();
    }

    private async Task LoadLocationsForSelectedWarehouseAsync()
    {
        if (WarehousePicker.SelectedItem is not CatalogWarehouse wh)
        {
            _locations = [];
            LocationPicker.ItemsSource = null;
            LocationPicker.SelectedIndex = -1;
            return;
        }

        var locs = await _catalog.GetLocationsAsync(wh.Id);
        _locations = locs.ToList();

        _suppressPickerEvents = true;
        try
        {
            LocationPicker.ItemsSource = _locations;
            LocationPicker.SelectedIndex = _locations.Count > 0 ? 0 : -1;
        }
        finally
        {
            _suppressPickerEvents = false;
        }
    }

    private async Task RefreshSummaryAsync()
    {
        if (WarehousePicker.SelectedItem is not CatalogWarehouse wh
            || LocationPicker.SelectedItem is not CatalogLocation loc
            || ItemPicker.SelectedItem is not CatalogItem item)
        {
            ClearSummaryLabels();
            return;
        }

        var summary = await _catalog.GetSummaryAsync(wh.Id, loc.Id, item.Id);
        if (summary is null)
        {
            ClearSummaryLabels();
            return;
        }

        SumItem.Text = $"{summary.ItemName} ({summary.ItemSku})";
        SumOnHand.Text = FormatQty(summary.OnHand);

        if (summary.Min is int mn && summary.Max is int mx)
            SumMinMax.Text = $"{mn} / {mx}";
        else
            SumMinMax.Text = "—";

        ApplyMinMaxProgress(summary);
        ApplyStatusPill(summary.Status);
    }

    private static string FormatQty(decimal value) =>
        value == decimal.Truncate(value)
            ? decimal.Truncate(value).ToString(CultureInfo.CurrentCulture)
            : value.ToString("0.##", CultureInfo.CurrentCulture);

    private void ApplyMinMaxProgress(CatalogSummary summary)
    {
        if (summary.Min is int mn && summary.Max is int mx && mx > mn)
        {
            MinMaxProgress.IsVisible = true;
            var span = Math.Max(1, mx - mn);
            var pos = (double)(summary.OnHand - mn) / span;
            MinMaxProgress.Progress = Math.Clamp(pos, 0, 1);
        }
        else
        {
            MinMaxProgress.IsVisible = false;
            MinMaxProgress.Progress = 0;
        }
    }

    private void ApplyStatusPill(string status)
    {
        var s = (status ?? "").Trim();
        StatusPillLabel.Text = string.IsNullOrEmpty(s) ? "—" : s;

        Color bg;
        Color fg;
        if (s.Contains("Below", StringComparison.OrdinalIgnoreCase))
        {
            bg = Color.FromArgb("#713F12");
            fg = Color.FromArgb("#FCD34D");
        }
        else if (s.Contains("Above", StringComparison.OrdinalIgnoreCase))
        {
            bg = Color.FromArgb("#7F1D1D");
            fg = Color.FromArgb("#FECACA");
        }
        else
        {
            bg = Color.FromArgb("#14532D");
            fg = Color.FromArgb("#86EFAC");
        }

        StatusPillBorder.BackgroundColor = bg;
        StatusPillLabel.TextColor = fg;
    }

    private void ClearSummaryLabels()
    {
        SumItem.Text = "—";
        SumOnHand.Text = "—";
        SumMinMax.Text = "—";
        MinMaxProgress.IsVisible = false;
        MinMaxProgress.Progress = 0;
        ApplyStatusPill("");
    }

    private async void OnScanCompleted(object? sender, EventArgs e)
    {
        var raw = (ScanEntry.Text ?? "").Trim();
        if (string.IsNullOrEmpty(raw))
            return;

        if (raw.Length <= MaxLocationCodeChars)
        {
            var locHit = _locations.FirstOrDefault(l =>
                string.Equals(l.Code, raw, StringComparison.OrdinalIgnoreCase));
            if (locHit is not null)
            {
                _suppressPickerEvents = true;
                try
                {
                    LocationPicker.SelectedItem = locHit;
                }
                finally
                {
                    _suppressPickerEvents = false;
                }

                ScanEntry.Text = "";
                await RefreshSummaryAsync();
                return;
            }
        }

        var itemHit = FindItemByScanCode(raw);
        if (itemHit is not null)
        {
            _suppressPickerEvents = true;
            try
            {
                ItemPicker.SelectedItem = itemHit;
            }
            finally
            {
                _suppressPickerEvents = false;
            }

            ScanEntry.Text = "";
            await RefreshSummaryAsync();
            return;
        }

        await FlashScanHintAsync("Unknown code");
    }

    private CatalogItem? FindItemByScanCode(string code)
    {
        foreach (var i in _items)
        {
            if (string.Equals(i.Sku, code, StringComparison.OrdinalIgnoreCase))
                return i;

            if (!string.IsNullOrWhiteSpace(i.ArticleNumber)
                && string.Equals(i.ArticleNumber.Trim(), code, StringComparison.OrdinalIgnoreCase))
                return i;

            foreach (var line in SplitBarcodeLines(i.Barcodes))
            {
                if (string.Equals(line, code, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
        }

        return null;
    }

    private static IEnumerable<string> SplitBarcodeLines(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
            yield break;

        foreach (var line in raw.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            var t = line.Trim();
            if (t.Length > 0)
                yield return t;
        }
    }

    private async Task FlashScanHintAsync(string message)
    {
        _scanMessageCts?.Cancel();
        _scanMessageCts = new CancellationTokenSource();
        var token = _scanMessageCts.Token;

        ScanHintLabel.Text = message;
        ScanHintLabel.IsVisible = true;

        try
        {
            await Task.Delay(2400, token);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        ScanHintLabel.IsVisible = false;
        ScanHintLabel.Text = "";
    }

    private void OnQtyMinusClicked(object? sender, EventArgs e)
    {
        _qty = Math.Max(1, _qty - 1);
        QtyLabel.Text = _qty.ToString(CultureInfo.CurrentCulture);
    }

    private void OnQtyPlusClicked(object? sender, EventArgs e)
    {
        _qty = Math.Min(9999, _qty + 1);
        QtyLabel.Text = _qty.ToString(CultureInfo.CurrentCulture);
    }

    private async void OnSyncClicked(object? sender, EventArgs e)
    {
        var result = await _catalogSync.SyncAsync();
        await DisplayAlertAsync(result.Success ? "Sync" : "Sync failed", result.Message, "OK");
        if (result.Success)
            await ReloadCatalogAfterSyncAsync();
    }

    private async void OnResetClicked(object? sender, EventArgs e)
    {
        _qty = 1;
        QtyLabel.Text = "1";
        ScanEntry.Text = "";

        _suppressPickerEvents = true;
        try
        {
            WarehousePicker.SelectedIndex = _warehouses.Count > 0 ? 0 : -1;
            ItemPicker.SelectedIndex = _items.Count > 0 ? 0 : -1;
        }
        finally
        {
            _suppressPickerEvents = false;
        }

        await LoadLocationsForSelectedWarehouseAsync();
        await RefreshSummaryAsync();
        Dispatcher.Dispatch(() => ScanEntry.Focus());
    }

    private async void OnInboundClicked(object? sender, EventArgs e) =>
        await SubmitMovementAsync("IN");

    private async void OnOutboundClicked(object? sender, EventArgs e) =>
        await SubmitMovementAsync("OUT");

    private async Task SubmitMovementAsync(string direction)
    {
        if (LocationPicker.SelectedItem is not CatalogLocation loc
            || ItemPicker.SelectedItem is not CatalogItem item)
        {
            await DisplayAlertAsync("Incomplete", "Select a location and an item first.", "OK");
            return;
        }

        if (_qty <= 0)
        {
            await DisplayAlertAsync("Quantity", "Quantity must be greater than zero.", "OK");
            return;
        }

        var qty = (decimal)_qty;
        var request = new StockMovementRequest(loc.Code, item.Sku, qty, direction);
        var result = await _stockClient.SubmitAsync(request);
        await DisplayAlertAsync(result.Success ? "Success" : "Error", result.Message, "OK");
        if (result.Success)
        {
            _qty = 1;
            QtyLabel.Text = "1";
            await RefreshSummaryAsync();
        }
    }
}
