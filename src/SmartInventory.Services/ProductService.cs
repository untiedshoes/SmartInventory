using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventory.Core.Entities;
using SmartInventory.Core.Interfaces;
using SmartInventory.Data;

namespace SmartInventory.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(InventoryDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all products.");
            // Only include Category, not Category.Products
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
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
            if (product == null) throw new KeyNotFoundException("Product not found.");
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // ---------------------------
        // New paginated / filtered method
        // ---------------------------
        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
            int page, int pageSize, Guid? categoryId = null, string? search = null)
        {
            var query = _context.Products
                .Include(p => p.Category) // Include Category, but not Category.Products
                .AsNoTracking()
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        // ---------------------------
        // Top products for dashboard
        // ---------------------------
        public async Task<IEnumerable<Product>> GetTopAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Quantity) // Or any metric you want
                .Take(count)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}