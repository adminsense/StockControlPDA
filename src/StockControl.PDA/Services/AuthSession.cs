namespace StockControl.PDA.Services;

public sealed class AuthSession : IAuthSession
{
    private const string TokenKey = "sc_auth_token";
    private const string NameKey = "sc_auth_name";

    public async Task<bool> IsLoggedInAsync()
        => !string.IsNullOrWhiteSpace(await GetTokenAsync());

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await SecureStorage.Default.GetAsync(TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetDisplayNameAsync()
    {
        try
        {
            return await SecureStorage.Default.GetAsync(NameKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveAsync(string token, string displayName)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
        await SecureStorage.Default.SetAsync(NameKey, displayName);
    }

    public async Task ClearAsync()
    {
        try
        {
            SecureStorage.Default.Remove(TokenKey);
            SecureStorage.Default.Remove(NameKey);
        }
        catch
        {
            // ignore
        }

        await Task.CompletedTask;
    }
}
