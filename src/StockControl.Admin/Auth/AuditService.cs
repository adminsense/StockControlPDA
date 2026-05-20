using Microsoft.EntityFrameworkCore;
using StockControl.Admin.Data;

namespace StockControl.Admin.Auth;

public sealed record AuditEntry(
    string Action,
    string EntityName,
    int? EntityId = null,
    int? UserId = null,
    string? Username = null,
    bool Success = true,
    string Severity = "Information",
    string? IpAddress = null,
    string? OldValues = null,
    string? NewValues = null,
    string? ErrorMessage = null);

public interface IAuditService
{
    Task LogAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}

public sealed class AuditService(IDbContextFactory<AppDbContext> dbFactory) : IAuditService
{
    public async Task LogAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        db.AuditLogs.Add(new AuditLog
        {
            Timestamp = DateTimeOffset.UtcNow,
            Action = entry.Action,
            EntityName = entry.EntityName,
            EntityId = entry.EntityId,
            UserId = entry.UserId,
            Username = (entry.Username ?? "").Trim(),
            Success = entry.Success,
            Severity = entry.Severity,
            IpAddress = entry.IpAddress,
            OldValues = entry.OldValues,
            NewValues = entry.NewValues,
            ErrorMessage = entry.ErrorMessage
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}

public static class AuditActions
{
    public const string Login = "Login";
    public const string LoginFailed = "LoginFailed";
    public const string Logout = "Logout";
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Activate = "Activate";
    public const string Deactivate = "Deactivate";
    public const string StockIn = "StockIn";
    public const string StockOut = "StockOut";
    public const string Sync = "Sync";

    public static string? BadgeClass(string action) => action switch
    {
        Login => "audit-badge-login",
        Logout => "audit-badge-logout",
        LoginFailed => "audit-badge-fail",
        Create => "audit-badge-create",
        Update => "audit-badge-update",
        Activate => "audit-badge-create",
        Deactivate => "audit-badge-deactivate",
        StockIn => "audit-badge-stock-in",
        StockOut => "audit-badge-stock-out",
        Sync => "audit-badge-sync",
        _ => "audit-badge-default"
    };
}
