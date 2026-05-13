namespace StockControl.Admin.Validation;

public static class UsersSaveValidation
{
    public const int MaxUsernameLength = 50;
    public const int MaxNameLength = 50;

    /// <param name="username">Already trimmed.</param>
    /// <param name="name">Already trimmed.</param>
    public static string? ValidateSave(string username, string name)
    {
        if (string.IsNullOrWhiteSpace(username))
            return "Username is required.";
        if (username.Length > MaxUsernameLength)
            return "Username must be 50 characters or fewer.";
        if (string.IsNullOrWhiteSpace(name))
            return "Name is required.";
        if (name.Length > MaxNameLength)
            return "Name must be 50 characters or fewer.";
        return null;
    }
}
