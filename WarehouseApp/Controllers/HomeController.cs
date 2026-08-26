using Microsoft.AspNetCore.Mvc;
using WarehouseApp.Services;
using WarehouseApp.ViewModels;

namespace WarehouseApp.Controllers;

public class HomeController(IInventoryService inventoryService, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        try
        {
            var dashboard = await inventoryService.GetDashboardAsync();
            return View(new DashboardViewModel
            {
                TotalProducts = dashboard.TotalProducts, TotalUnitsInStock = dashboard.TotalUnitsInStock,
                TotalReceivedToday = dashboard.TotalReceivedToday, TotalWithdrawnToday = dashboard.TotalWithdrawnToday,
                LowStockProducts = dashboard.LowStockProducts, RecentTransactions = dashboard.RecentTransactions
            });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to load dashboard.");
            TempData["Error"] = "Unable to load dashboard data. Please try again.";
            return View(new DashboardViewModel());
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
