using System.Text.Json.Serialization;

namespace StockControl.PDA.Services;

public sealed record StockMovementRequest(
    [property: JsonPropertyName("locationCode")] string LocationCode,
    [property: JsonPropertyName("itemCode")] string ItemCode,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("direction")] string Direction);

public sealed class MovementSubmitResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";

    public static MovementSubmitResult Ok(string message = "Saved.") =>
        new() { Success = true, Message = message };

    public static MovementSubmitResult Fail(string message) =>
        new() { Success = false, Message = message };
}
