using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Entities;
using SmartInventory.Data;
using SmartInventory.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace SmartInventory.Services.Tests
{
    public class ProductServiceTests
    {
        /// <summary>
        /// Creates a fresh in-memory DbContext for each test.
        /// </summary>
        private InventoryDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            return new InventoryDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddProduct()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var service = new ProductService(context, NullLogger<ProductService>.Instance);
            var product = new Product { Name = "Test Product", Quantity = 5, Category = category };

            // Act
            var created = await service.CreateAsync(product);

            // Assert
            Assert.NotNull(created);
            Assert.Equal("Test Product", created.Name);
            Assert.Equal(1, context.Products.Count());
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenQuantityNegative()
        {
            var context = GetInMemoryDbContext();
            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var service = new ProductService(context, NullLogger<ProductService>.Instance);
            var product = new Product { Name = "Invalid", Quantity = -1, Category = category };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.CreateAsync(product));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            var context = GetInMemoryDbContext();

            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            context.Products.Add(new Product { Name = "P1", Quantity = 2, Category = category });
            context.Products.Add(new Product { Name = "P2", Quantity = 3, Category = category });
            await context.SaveChangesAsync();

            var service = new ProductService(context, NullLogger<ProductService>.Instance);

            var all = await service.GetAllAsync();

            Assert.Equal(2, all.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product { Name = "P1", Quantity = 1, Category = category };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var service = new ProductService(context, NullLogger<ProductService>.Instance);
            var fetched = await service.GetByIdAsync(product.Id);

            Assert.NotNull(fetched);
            Assert.Equal("P1", fetched.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var service = new ProductService(context, NullLogger<ProductService>.Instance);

            var result = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            var context = GetInMemoryDbContext();
            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product { Name = "OldName", Quantity = 1, Category = category };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var service = new ProductService(context, NullLogger<ProductService>.Instance);
            product.Name = "NewName";

            await service.UpdateAsync(product);

            var updated = await context.Products.FindAsync(product.Id);
            Assert.NotNull(updated);
            Assert.Equal("NewName", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product { Name = "ToDelete", Quantity = 1, Category = category };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var service = new ProductService(context, NullLogger<ProductService>.Instance);
            await service.DeleteAsync(product.Id);

            Assert.Empty(context.Products);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var service = new ProductService(context, NullLogger<ProductService>.Instance);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await service.DeleteAsync(Guid.NewGuid()));
        }
    }
}