using SmartInventory.Core.Entities;
using SmartInventory.Core.Interfaces;
using SmartInventory.Core.Models; // For PagedResult<T>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartInventory.Services
{
    public class FakeProductService : IProductService
    {
        private readonly List<Category> _categories;
        private readonly List<Product> _products;

        public FakeProductService()
        {
            _categories = new List<Category>();
            _products = new List<Product>();

            // Create 15 categories
            for (int i = 1; i <= 15; i++)
            {
                _categories.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = $"Category {i}",
                    Description = $"Description for Category {i}"
                });
            }

            var random = new Random();

            // Create 10 products per category = 150 products
            foreach (var category in _categories)
            {
                for (int i = 1; i <= 10; i++)
                {
                    _products.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{category.Name} - Product {i}",
                        Description = $"Description for {category.Name} Product {i}",
                        Quantity = random.Next(0, 200),
                        Price = Math.Round((decimal)(random.NextDouble() * 500 + 10), 2),
                        CategoryId = category.Id
                    });
                }
            }
        }

        public Task<IEnumerable<Product>> GetAllAsync() =>
            Task.FromResult(_products.AsEnumerable());

        public Task<Product?> GetByIdAsync(Guid id) =>
            Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<Product> CreateAsync(Product product)
        {
            product.Id = Guid.NewGuid();
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task UpdateAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing is not null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Quantity = product.Quantity;
                existing.Price = product.Price;
                existing.CategoryId = product.CategoryId;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
                _products.Remove(product);
            return Task.CompletedTask;
        }

        // ✅ Implement GetPagedAsync with filtering and search
        public Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? categoryId, string? search)
        {
            var query = _products.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Task.FromResult((items.AsEnumerable(), totalCount));
        }
    }
}