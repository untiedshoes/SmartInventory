using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Entities;

namespace SmartInventory.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var electronicsId = Guid.NewGuid();
        var booksId = Guid.NewGuid();

        modelBuilder.Entity<Category>().HasData(
        new Category { Id = electronicsId, Name = "Electronics" },
        new Category { Id = booksId, Name = "Books" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = Guid.NewGuid(), Name = "Laptop", Quantity = 5, CategoryId = electronicsId },
            new Product { Id = Guid.NewGuid(), Name = "C# Book", Quantity = 10, CategoryId = booksId }
        );
    }
}
