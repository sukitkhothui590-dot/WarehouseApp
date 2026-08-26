using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.ViewModels;

public class ProductFormViewModel
{
    public int Id { get; set; }
    [Required, StringLength(32), Display(Name = "Product code")]
    public string Code { get; set; } = string.Empty;
    [Required, StringLength(160)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(32)] public string Unit { get; set; } = string.Empty;
}
