using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartInventory.Core.Entities;

namespace SmartInventory.Tests.Services
{
    /// <summary>
    /// Fake in-memory product service for testing without a real database.
    /// Mirrors ProductService behavior for unit testing.
    /// </summary>
    public class FakeProductService
    {
        // In-memory storage
        private readonly List<Product> _products;
        private readonly List<Category> _categories;

        public FakeProductService()
        {
            // Seed categories
            _categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Category 1" },
                new Category { Id = Guid.NewGuid(), Name = "Category 2" }
            };

            // Seed products
            // Use zero-padded names ("Product01") to fix ordering in pagination tests
            _products = new List<Product>();
            for (int i = 1; i <= 10; i++)
            {
                _products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product{i:D2}",                // D2 ensures Product01 < Product02 < ... < Product10
                    Description = i % 2 == 0 ? "Foo" : "Bar", // Alternating descriptions for search tests
                    Quantity = i * 10,                        // Deterministic quantity for top product tests
                    CategoryId = i <= 5 ? _categories[0].Id : _categories[1].Id, // First 5 -> Cat1, next 5 -> Cat2
                    Category = i <= 5 ? _categories[0] : _categories[1]
                });
            }
        }

        // -----------------------------
        // Simulate async EF Core queries
        // -----------------------------
        public Task<IEnumerable<Product>> GetAllAsync()
        {
            // Return a copy to prevent external modifications
            return Task.FromResult(_products.AsEnumerable());
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<Product> CreateAsync(Product product)
        {
            // Assign a new ID to simulate DB-generated IDs
            product.Id = Guid.NewGuid();

            // Assign category reference if provided
            if (product.Category != null)
            {
                product.CategoryId = product.Category.Id;
            }

            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task UpdateAsync(Product product)
        {
            // Find existing product by ID
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index >= 0)
            {
                _products[index] = product; // Replace
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _products.RemoveAll(p => p.Id == id);
            return Task.CompletedTask;
        }

        // -----------------------------
        // Pagination + Filtering
        // -----------------------------
        public Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
            int page, int pageSize, Guid? categoryId = null, string? search = null)
        {
            var query = _products.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            var totalCount = query.Count();

            var data = query
                .OrderBy(p => p.Name)                     // Important: deterministic order for pagination tests
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult((data.AsEnumerable(), totalCount));
        }

        // -----------------------------
        // Top products for dashboard
        // -----------------------------
        public Task<IEnumerable<Product>> GetTopAsync(int count)
        {
            var topProducts = _products
                .OrderByDescending(p => p.Quantity)      // High quantity first
                .Take(count)
                .ToList();

            return Task.FromResult(topProducts.AsEnumerable());
        }
    }
}