using Microsoft.EntityFrameworkCore;
using WarehouseApp.Models;

namespace WarehouseApp.Data;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(product => product.Code).IsUnique();
            entity.Property(product => product.Code).IsRequired();
            entity.Property(product => product.Name).IsRequired();
            entity.Property(product => product.Unit).IsRequired();
            entity.HasMany(product => product.Transactions).WithOne(transaction => transaction.Product)
                .HasForeignKey(transaction => transaction.ProductId).OnDelete(DeleteBehavior.Restrict);
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new Product { Id = 1, Code = "P001", Name = "Wireless Mouse", Unit = "pcs", Quantity = 0, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 2, Code = "P002", Name = "Mechanical Keyboard", Unit = "pcs", Quantity = 0, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 3, Code = "P003", Name = "USB-C Cable", Unit = "pcs", Quantity = 0, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 4, Code = "P004", Name = "Monitor 24 inch", Unit = "pcs", Quantity = 0, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Product { Id = 5, Code = "P005", Name = "Notebook Stand", Unit = "pcs", Quantity = 0, CreatedAt = seedDate, UpdatedAt = seedDate });
        });
        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.Property(transaction => transaction.TransactionType).HasConversion<string>();
            entity.Property(transaction => transaction.Note).HasMaxLength(500);
        });
    }
}
