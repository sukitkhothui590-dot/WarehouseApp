using WarehouseApp.Models;

namespace WarehouseApp.Services;

public interface IInventoryService
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string? search = null);
    Task<DashboardData> GetDashboardAsync();
    Task<StockMovementResult> ReceiveStockAsync(int productId, int quantity, string? note);
    Task<StockMovementResult> WithdrawStockAsync(int productId, int quantity, string? note);
    Task<IReadOnlyList<StockTransaction>> GetTransactionsAsync(TransactionType? type = null, int? productId = null);
    Task<Product?> GetProductDetailAsync(int productId);
}
