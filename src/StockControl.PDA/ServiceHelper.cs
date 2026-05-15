namespace StockControl.PDA;

/// <summary>Provides app-wide service resolution after <see cref="MauiApp"/> is built (pages created from XAML do not get constructor DI).</summary>
public static class ServiceHelper
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static void Init(IServiceProvider services) => Services = services;
}
