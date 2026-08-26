using WarehouseApp.Models;

namespace WarehouseApp.ViewModels;

public class InventoryViewModel
{
    public string? Search { get; init; }
    public IReadOnlyList<Product> Products { get; init; } = [];
}
