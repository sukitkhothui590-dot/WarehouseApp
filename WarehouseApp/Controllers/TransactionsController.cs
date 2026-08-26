using Microsoft.AspNetCore.Mvc;
using WarehouseApp.Models;
using WarehouseApp.Services;
using WarehouseApp.ViewModels;

namespace WarehouseApp.Controllers;

public class TransactionsController(IInventoryService inventoryService) : Controller
{
    public async Task<IActionResult> Index(string filter = "all", int? productId = null)
    {
        var type = filter.ToLowerInvariant() switch
        {
            "receive" => TransactionType.IN,
            "withdraw" => TransactionType.OUT,
            _ => (TransactionType?)null
        };
        return View(new TransactionsViewModel
        {
            Filter = filter, ProductId = productId, Products = await inventoryService.GetProductsAsync(),
            Transactions = await inventoryService.GetTransactionsAsync(type, productId)
        });
    }
}
