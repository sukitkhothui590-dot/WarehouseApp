using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseApp.Models;

namespace WarehouseApp.ViewModels;

public class StockMovementViewModel
{
    [Required, Display(Name = "Product")]
    public int ProductId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }
    [StringLength(500)] public string? Note { get; set; }
    public IEnumerable<SelectListItem> ProductOptions { get; set; } = [];
    public IReadOnlyList<Product> Products { get; set; } = [];
}
