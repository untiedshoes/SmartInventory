using System;
using System.Linq;
using System.Threading.Tasks;
using SmartInventory.Core.Entities;
using SmartInventory.Services;
using Xunit;

namespace SmartInventory.Tests.Services
{
    public class FakeProductServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsSeededProducts()
        {
            var svc = new FakeProductService();
            var all = (await svc.GetAllAsync()).ToList();

            Assert.True(all.Count >= 2);
            Assert.Contains(all, p => p.Name == "Product 1");
            Assert.Contains(all, p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProduct_WhenExists()
        {
            var svc = new FakeProductService();
            var all = (await svc.GetAllAsync()).ToList();
            var id = all.First().Id;

            var fetched = await svc.GetByIdAsync(id);

            Assert.NotNull(fetched);
            Assert.Equal(all.First().Name, fetched.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            var svc = new FakeProductService();

            var result = await svc.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_AddsProduct_AndAssignsId()
        {
            var svc = new FakeProductService();
            var product = new Product { Name = "New", Quantity = 3 };

            var created = await svc.CreateAsync(product);

            Assert.NotEqual(Guid.Empty, created.Id);
            var all = (await svc.GetAllAsync()).ToList();
            Assert.Contains(all, p => p.Id == created.Id && p.Name == "New");
        }

        [Fact]
        public async Task UpdateAsync_UpdatesExistingProduct()
        {
            var svc = new FakeProductService();
            var product = (await svc.GetAllAsync()).First();
            var originalName = product.Name;
            product.Name = "UpdatedName";

            await svc.UpdateAsync(product);

            var fetched = await svc.GetByIdAsync(product.Id);
            Assert.NotNull(fetched);
            Assert.Equal("UpdatedName", fetched.Name);
            Assert.NotEqual(originalName, fetched.Name);
        }

        [Fact]
        public async Task DeleteAsync_RemovesProduct()
        {
            var svc = new FakeProductService();
            var product = (await svc.GetAllAsync()).First();

            await svc.DeleteAsync(product.Id);

            var fetched = await svc.GetByIdAsync(product.Id);
            Assert.Null(fetched);
        }
    }
}
