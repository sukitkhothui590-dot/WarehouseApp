using Microsoft.AspNetCore.Mvc;
using WarehouseApp.Services;
using WarehouseApp.ViewModels;

namespace WarehouseApp.Controllers;

public class InventoryController(IInventoryService inventoryService) : Controller
{
    public async Task<IActionResult> Index(string? search) =>
        View(new InventoryViewModel { Search = search, Products = await inventoryService.GetProductsAsync(search) });
}
