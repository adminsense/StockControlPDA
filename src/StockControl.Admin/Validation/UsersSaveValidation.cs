using StockControl.Admin.Data;

namespace StockControl.Admin.Validation;

public static class UsersSaveValidation
{
    public const int MaxUsernameLength = 50;
    public const int MaxNameLength = 50;
    public const int MinPasswordLength = 6;

    /// <param name="username">Already trimmed.</param>
    /// <param name="name">Already trimmed.</param>
    public static string? ValidateSave(string username, string name, UserRole role, bool isNew, string? password)
    {
        if (string.IsNullOrWhiteSpace(username))
            return "Username is required.";
        if (username.Length > MaxUsernameLength)
            return "Username must be 50 characters or fewer.";
        if (string.IsNullOrWhiteSpace(name))
            return "Name is required.";
        if (name.Length > MaxNameLength)
            return "Name must be 50 characters or fewer.";
        if (role is not (UserRole.Admin or UserRole.AdminPda))
            return "Role must be Admin (1) or Admin PDA (2).";
        if (isNew && string.IsNullOrWhiteSpace(password))
            return "Password is required for new users.";
        if (!string.IsNullOrWhiteSpace(password) && password.Trim().Length < MinPasswordLength)
            return $"Password must be at least {MinPasswordLength} characters.";
        return null;
    }

    public static string RoleLabel(UserRole role) => role switch
    {
        UserRole.Admin => "Admin (1)",
        UserRole.AdminPda => "Admin PDA (2)",
        _ => role.ToString()
    };
}
