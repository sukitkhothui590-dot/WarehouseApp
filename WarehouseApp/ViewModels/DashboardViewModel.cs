using WarehouseApp.Models;

namespace WarehouseApp.ViewModels;

public class DashboardViewModel
{
    public int TotalProducts { get; init; }
    public int TotalUnitsInStock { get; init; }
    public int TotalReceivedToday { get; init; }
    public int TotalWithdrawnToday { get; init; }
    public IReadOnlyList<Product> LowStockProducts { get; init; } = [];
    public IReadOnlyList<StockTransaction> RecentTransactions { get; init; } = [];
}
