using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockControl.PDA.Configuration;
using StockControl.PDA.Services;

namespace StockControl.PDA;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		var assembly = typeof(MauiProgram).Assembly;
		using (var stream = assembly.GetManifestResourceStream("StockControl.PDA.appsettings.json"))
		{
			if (stream is not null)
				builder.Configuration.AddJsonStream(stream);
		}

		builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.SectionName));
		void ConfigureApiBase(IServiceProvider sp, HttpClient client)
		{
			var baseUrl = sp.GetRequiredService<IOptions<ApiOptions>>().Value.BaseUrl?.Trim();
			if (!string.IsNullOrWhiteSpace(baseUrl) && Uri.TryCreate(baseUrl.TrimEnd('/') + "/", UriKind.Absolute, out var uri))
				client.BaseAddress = uri;
		}

		builder.Services.AddHttpClient<IStockMovementClient, StockMovementHttpClient>()
			.ConfigureHttpClient(ConfigureApiBase);
		builder.Services.AddHttpClient<ICatalogSyncClient, CatalogSyncHttpClient>()
			.ConfigureHttpClient(ConfigureApiBase);
		builder.Services.AddHttpClient<IMoveStockCatalogClient, MoveStockCatalogHttpClient>()
			.ConfigureHttpClient(ConfigureApiBase);

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		ServiceHelper.Init(app.Services);
		return app;
	}
}
