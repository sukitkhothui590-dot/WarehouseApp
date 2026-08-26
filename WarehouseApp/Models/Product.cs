using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseApp.Models;

public class Product
{
    public int Id { get; set; }
    [MaxLength(32)] public string Code { get; set; } = string.Empty;
    [MaxLength(160)] public string Name { get; set; } = string.Empty;
    [MaxLength(32)] public string Unit { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [NotMapped]
    public string ImageFileName => Code.ToUpperInvariant() switch
    {
        "P001" => "wireless-mouse.jpg",
        "P002" => "mechanical-keyboard.jpg",
        "P003" => "usb-c-cable.jpg",
        "P004" => "monitor.jpg",
        "P005" => "notebook-stand.jpg",
        _ => "default-product.svg"
    };
    public ICollection<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
}
