using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;
using WarehouseApp.Models;
using WarehouseApp.Services;
using WarehouseApp.ViewModels;

namespace WarehouseApp.Controllers;

public class ProductsController(WarehouseDbContext db, IInventoryService inventoryService) : Controller
{
    public async Task<IActionResult> Index(string? search, string? status)
    {
        ViewData["Search"] = search;
        ViewData["Status"] = status;
        var products = await inventoryService.GetProductsAsync(search);
        if (string.Equals(status, "in-stock", StringComparison.OrdinalIgnoreCase))
            products = products.Where(product => product.Quantity > 5).ToList();
        else if (string.Equals(status, "low-stock", StringComparison.OrdinalIgnoreCase))
            products = products.Where(product => product.Quantity > 0 && product.Quantity <= 5).ToList();
        else if (string.Equals(status, "out-of-stock", StringComparison.OrdinalIgnoreCase))
            products = products.Where(product => product.Quantity == 0).ToList();
        return View(products);
    }
    public IActionResult Create() => View(new ProductFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var code = model.Code.Trim();
        if (await db.Products.AnyAsync(product => product.Code == code))
        {
            ModelState.AddModelError(nameof(model.Code), "Product code already exists.");
            return View(model);
        }
        var now = DateTime.UtcNow;
        db.Products.Add(new Product { Code = code, Name = model.Name.Trim(), Unit = model.Unit.Trim(), Quantity = 0, CreatedAt = now, UpdatedAt = now });
        await db.SaveChangesAsync();
        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await db.Products.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
        return product is null ? NotFound() : View(new ProductFormViewModel { Id = product.Id, Code = product.Code, Name = product.Name, Unit = product.Unit });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var product = await db.Products.SingleOrDefaultAsync(item => item.Id == model.Id);
        if (product is null) return NotFound();
        var code = model.Code.Trim();
        if (await db.Products.AnyAsync(item => item.Id != model.Id && item.Code == code))
        {
            ModelState.AddModelError(nameof(model.Code), "Product code already exists.");
            return View(model);
        }
        product.Code = code; product.Name = model.Name.Trim(); product.Unit = model.Unit.Trim(); product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await inventoryService.GetProductDetailAsync(id);
        return product is null ? NotFound() : View(new ProductDetailViewModel { Product = product, Transactions = product.Transactions.ToList() });
    }
}
