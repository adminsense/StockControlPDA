namespace StockControl.PDA.Services;

public interface IAuthSession
{
    Task<bool> IsLoggedInAsync();
    Task<string?> GetTokenAsync();
    Task<string?> GetDisplayNameAsync();
    Task SaveAsync(string token, string displayName);
    Task ClearAsync();
}
