using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartInventory.Core.Entities;
using SmartInventory.Core.Interfaces;
using SmartInventory.Services.DTOs;

namespace SmartInventory.Services
{
    /// <summary>
    /// A fake in-memory product service for testing and dev mode.
    /// Implements IProductService and provides DTO methods for the controller.
    /// </summary>
    public class FakeProductService : IProductService
    {
        private readonly List<Category> _categories;
        private readonly List<Product> _products;

        public FakeProductService()
        {
            _categories = new List<Category>();
            _products = new List<Product>();

            // Create 2 categories
            for (int i = 1; i <= 2; i++)
            {
                _categories.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = $"Category {i}",
                    Description = $"Description for Category {i}"
                });
            }

            // Create 5 products per category = 10 products
            for (int i = 1; i <= 5; i++)
            {
                _products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product{i:D2}",
                    Description = i % 2 == 0 ? "Foo" : "Bar",
                    Quantity = i * 10,
                    CategoryId = _categories[0].Id,
                    Category = _categories[0]
                });
            }

            for (int i = 6; i <= 10; i++)
            {
                _products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product{i:D2}",
                    Description = i % 2 == 0 ? "Foo" : "Bar",
                    Quantity = i * 10,
                    CategoryId = _categories[1].Id,
                    Category = _categories[1]
                });
            }
        }

        // ---------------------------
        // IProductService CRUD Methods
        // ---------------------------

        public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult(_products.AsEnumerable());

        public Task<Product?> GetByIdAsync(Guid id) =>
            Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<Product> CreateAsync(Product product)
        {
            product.Id = Guid.NewGuid();
            if (product.Category != null)
                product.CategoryId = product.Category.Id;

            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task UpdateAsync(Product product)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index >= 0) _products[index] = product;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _products.RemoveAll(p => p.Id == id);
            return Task.CompletedTask;
        }

        // ---------------------------
        // Paging & Filtering
        // ---------------------------

        public Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
            int page, int pageSize, Guid? categoryId = null, string? search = null)
        {
            var query = _products.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => (p.Name + p.Description).Contains(search, StringComparison.OrdinalIgnoreCase));

            var totalCount = query.Count();

            var pageItems = query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult((pageItems.AsEnumerable(), totalCount));
        }

        // ---------------------------
        // Top-N Products
        // ---------------------------

        public Task<IEnumerable<Product>> GetTopAsync(int count)
        {
            var topProducts = _products
                .OrderByDescending(p => p.Quantity)
                .Take(count)
                .ToList();

            return Task.FromResult(topProducts.AsEnumerable());
        }

        // ---------------------------
        // DTO Methods for Controllers
        // ---------------------------

        public Task<IEnumerable<ProductDto>> GetAllDtosAsync()
        {
            var dtos = _products.Select(ToDto);
            return Task.FromResult(dtos);
        }

        public Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetPagedDtosAsync(
            int page, int pageSize, Guid? categoryId = null, string? search = null)
        {
            var query = _products.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => (p.Name + p.Description).Contains(search, StringComparison.OrdinalIgnoreCase));

            var totalCount = query.Count();

            var pageItems = query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto);

            return Task.FromResult((pageItems, totalCount));
        }

        public Task<IEnumerable<ProductDto>> GetTopDtosAsync(int count)
        {
            var top = _products
                .OrderByDescending(p => p.Quantity)
                .Take(count)
                .Select(ToDto);

            return Task.FromResult(top);
        }

        public Task<ProductDto?> GetByIdDtoAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product is null ? null : ToDto(product));
        }

        // ---------------------------
        // Helper
        // ---------------------------

        private static ProductDto ToDto(Product p) =>
            new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Quantity = p.Quantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            };
    }
}