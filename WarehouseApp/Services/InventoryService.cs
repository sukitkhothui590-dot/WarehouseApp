using System.Data;
using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;
using WarehouseApp.Models;

namespace WarehouseApp.Services;

public sealed record StockMovementResult(Product Product, StockTransaction Transaction);
public sealed record DashboardData(int TotalProducts, int TotalUnitsInStock, int TotalReceivedToday, int TotalWithdrawnToday, IReadOnlyList<Product> LowStockProducts, IReadOnlyList<StockTransaction> RecentTransactions);

public class InventoryService(WarehouseDbContext db, ILogger<InventoryService> logger) : IInventoryService
{
    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? search = null)
    {
        var query = db.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(product => product.Code.Contains(term) || product.Name.Contains(term));
        }
        return await query.OrderBy(product => product.Code).ToListAsync();
    }

    public async Task<DashboardData> GetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var todayTransactions = db.StockTransactions.Where(transaction => transaction.CreatedAt >= today);
        return new DashboardData(
            await db.Products.CountAsync(),
            await db.Products.SumAsync(product => (int?)product.Quantity) ?? 0,
            await todayTransactions.Where(transaction => transaction.TransactionType == TransactionType.IN).SumAsync(transaction => (int?)transaction.Quantity) ?? 0,
            await todayTransactions.Where(transaction => transaction.TransactionType == TransactionType.OUT).SumAsync(transaction => (int?)transaction.Quantity) ?? 0,
            await db.Products.AsNoTracking().Where(product => product.Quantity <= 5).OrderBy(product => product.Quantity).ThenBy(product => product.Code).ToListAsync(),
            await db.StockTransactions.AsNoTracking().Include(transaction => transaction.Product).OrderByDescending(transaction => transaction.CreatedAt).Take(8).ToListAsync());
    }

    public Task<StockMovementResult> ReceiveStockAsync(int productId, int quantity, string? note) => MoveStockAsync(productId, quantity, TransactionType.IN, note);
    public Task<StockMovementResult> WithdrawStockAsync(int productId, int quantity, string? note) => MoveStockAsync(productId, quantity, TransactionType.OUT, note);

    public async Task<IReadOnlyList<StockTransaction>> GetTransactionsAsync(TransactionType? type = null, int? productId = null)
    {
        var query = db.StockTransactions.AsNoTracking().Include(transaction => transaction.Product).AsQueryable();
        if (type.HasValue) query = query.Where(transaction => transaction.TransactionType == type.Value);
        if (productId.HasValue) query = query.Where(transaction => transaction.ProductId == productId.Value);
        return await query.OrderByDescending(transaction => transaction.CreatedAt).ThenByDescending(transaction => transaction.Id).ToListAsync();
    }

    public Task<Product?> GetProductDetailAsync(int productId) =>
        db.Products.AsNoTracking().Include(product => product.Transactions.OrderByDescending(transaction => transaction.CreatedAt)).SingleOrDefaultAsync(product => product.Id == productId);

    private async Task<StockMovementResult> MoveStockAsync(int productId, int quantity, TransactionType type, string? note)
    {
        if (quantity <= 0) throw new InventoryOperationException("Quantity must be greater than zero.");
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var product = await db.Products.SingleOrDefaultAsync(item => item.Id == productId);
            if (product is null) throw new InventoryOperationException("The selected product was not found.");
            if (type == TransactionType.OUT && quantity > product.Quantity)
                throw new InventoryOperationException($"Insufficient stock. Current stock: {product.Quantity}; requested quantity: {quantity}.");
            var before = product.Quantity;
            var after = type == TransactionType.IN ? before + quantity : before - quantity;
            if (after < 0) throw new InventoryOperationException("Stock cannot be negative.");
            product.Quantity = after;
            product.UpdatedAt = DateTime.UtcNow;
            var stockTransaction = new StockTransaction
            {
                ProductId = product.Id, TransactionType = type, Quantity = quantity, BalanceBefore = before,
                BalanceAfter = after, Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(), CreatedAt = DateTime.UtcNow
            };
            db.StockTransactions.Add(stockTransaction);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return new StockMovementResult(product, stockTransaction);
        }
        catch (InventoryOperationException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            logger.LogError(exception, "Stock movement failed for product {ProductId} and type {TransactionType}.", productId, type);
            throw;
        }
    }
}
