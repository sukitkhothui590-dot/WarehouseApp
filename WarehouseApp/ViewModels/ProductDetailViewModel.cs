using WarehouseApp.Models;

namespace WarehouseApp.ViewModels;

public class ProductDetailViewModel
{
    public Product Product { get; init; } = null!;
    public IReadOnlyList<StockTransaction> Transactions { get; init; } = [];
}
