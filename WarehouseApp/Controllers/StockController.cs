using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseApp.Services;
using WarehouseApp.ViewModels;

namespace WarehouseApp.Controllers;

public class StockController(IInventoryService inventoryService, ILogger<StockController> logger) : Controller
{
    [HttpGet] public Task<IActionResult> Receive() => MovementViewAsync("Receive stock");

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(StockMovementViewModel model)
    {
        if (!ModelState.IsValid) return await MovementViewAsync("Receive stock", model);
        try
        {
            await inventoryService.ReceiveStockAsync(model.ProductId, model.Quantity, model.Note);
            TempData["Success"] = "Stock received successfully.";
            return RedirectToAction("Index", "Inventory");
        }
        catch (InventoryOperationException exception) { ModelState.AddModelError(string.Empty, exception.Message); return await MovementViewAsync("Receive stock", model); }
        catch (Exception exception) { logger.LogError(exception, "Receive stock request failed."); ModelState.AddModelError(string.Empty, "Unable to receive stock. Please try again."); return await MovementViewAsync("Receive stock", model); }
    }

    [HttpGet] public Task<IActionResult> Withdraw() => MovementViewAsync("Withdraw stock");

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(StockMovementViewModel model)
    {
        if (!ModelState.IsValid) return await MovementViewAsync("Withdraw stock", model);
        try
        {
            await inventoryService.WithdrawStockAsync(model.ProductId, model.Quantity, model.Note);
            TempData["Success"] = "Stock withdrawn successfully.";
            return RedirectToAction("Index", "Inventory");
        }
        catch (InventoryOperationException exception) { ModelState.AddModelError(string.Empty, exception.Message); return await MovementViewAsync("Withdraw stock", model); }
        catch (Exception exception) { logger.LogError(exception, "Withdraw stock request failed."); ModelState.AddModelError(string.Empty, "Unable to withdraw stock. Please try again."); return await MovementViewAsync("Withdraw stock", model); }
    }

    private async Task<IActionResult> MovementViewAsync(string title, StockMovementViewModel? model = null)
    {
        var products = await inventoryService.GetProductsAsync();
        model ??= new StockMovementViewModel();
        model.Products = products;
        model.ProductOptions = products.Select(product => new SelectListItem($"{product.Code} — {product.Name} ({product.Quantity} {product.Unit})", product.Id.ToString()));
        ViewData["Title"] = title;
        ViewData["MovementType"] = title.StartsWith("Receive", StringComparison.Ordinal) ? "receive" : "withdraw";
        return View("Movement", model);
    }
}
