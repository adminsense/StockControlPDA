using Microsoft.Extensions.DependencyInjection;
using StockControl.PDA.Services;

namespace StockControl.PDA;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		var session = ServiceHelper.Services.GetRequiredService<IAuthSession>();
		if (await session.IsLoggedInAsync())
			await GoToAsync("//MainPage");
		else
			await GoToAsync("//Login");
	}
}
