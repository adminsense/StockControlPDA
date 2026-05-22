using Microsoft.Extensions.DependencyInjection;
using StockControl.PDA.Services;

namespace StockControl.PDA;

public partial class LoginPage : ContentPage
{
    private readonly IAuthApiClient _authApi =
        ServiceHelper.Services.GetRequiredService<IAuthApiClient>();

    private readonly IAuthSession _session =
        ServiceHelper.Services.GetRequiredService<IAuthSession>();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnSignInClicked(object? sender, EventArgs e) => await SignInAsync();

    private async void OnPasswordCompleted(object? sender, EventArgs e) => await SignInAsync();

    private async Task SignInAsync()
    {
        ErrorLabel.IsVisible = false;
        SignInButton.IsEnabled = false;
        try
        {
            var username = UsernameEntry.Text?.Trim() ?? "";
            var password = PasswordEntry.Text ?? "";
            var (ok, token, displayName, error) = await _authApi.LoginAsync(username, password);
            if (!ok || string.IsNullOrWhiteSpace(token))
            {
                ShowError(error ?? "Login failed.");
                return;
            }

            await _session.SaveAsync(token, displayName ?? username);
            await Shell.Current.GoToAsync("//MainPage");
        }
        finally
        {
            SignInButton.IsEnabled = true;
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}
