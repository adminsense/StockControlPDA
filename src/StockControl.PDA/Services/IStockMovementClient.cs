namespace StockControl.PDA.Services;

public interface IStockMovementClient
{
    Task<MovementSubmitResult> SubmitAsync(StockMovementRequest request, CancellationToken cancellationToken = default);
}
