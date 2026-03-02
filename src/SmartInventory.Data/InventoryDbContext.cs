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

        // Use stable GUIDs for seed data so migrations don't detect spurious changes
        var electronicsId = new Guid("b21735e3-2453-41a7-b63e-05c1677282a0");
        var booksId = new Guid("a772e954-fc01-4378-b759-435cbd973a37");

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = electronicsId, Name = "Electronics" },
            new Category { Id = booksId, Name = "Books" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = new Guid("cabf73d7-5dbc-413b-ad2b-df7d50b09d85"), Name = "Laptop", Quantity = 5, CategoryId = electronicsId },
            new Product { Id = new Guid("0151dacf-a48f-4499-b944-861c51e2a415"), Name = "C# Book", Quantity = 10, CategoryId = booksId }
        );
    }
}
