namespace WarehouseApp.Models;

public enum TransactionType { IN, OUT }

public class StockTransaction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public TransactionType TransactionType { get; set; }
    public int Quantity { get; set; }
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
