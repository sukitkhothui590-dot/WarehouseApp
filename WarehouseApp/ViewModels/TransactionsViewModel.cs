using WarehouseApp.Models;

namespace WarehouseApp.ViewModels;

public class TransactionsViewModel
{
    public string Filter { get; init; } = "all";
    public int? ProductId { get; init; }
    public IReadOnlyList<Product> Products { get; init; } = [];
    public IReadOnlyList<StockTransaction> Transactions { get; init; } = [];
}
