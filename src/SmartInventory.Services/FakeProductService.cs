using SmartInventory.Core.Entities;
using SmartInventory.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartInventory.Services
{
    public class FakeProductService : IProductService
    {
        private readonly List<Product> _products = new()
        {
            new Product { Id = Guid.NewGuid(), Name = "Product 1", Quantity = 10 },
            new Product { Id = Guid.NewGuid(), Name = "Product 2", Quantity = 5 }
        };

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.AsEnumerable());
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

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
                existing.Quantity = product.Quantity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is not null)
                _products.Remove(product);
            return Task.CompletedTask;
        }
    }
}