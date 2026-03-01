using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Entities;
using SmartInventory.Core.Interfaces;
using SmartInventory.Data;

namespace SmartInventory.Services;

/// <summary>
/// Contains business logic for products.
/// This is where rules and validation live.
/// </summary>
public class ProductService : IProductService
{
    private readonly InventoryDbContext _context;

    public ProductService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        // Include category for richer API response
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        // Business rule example
        if (product.Quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
            throw new KeyNotFoundException("Product not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}