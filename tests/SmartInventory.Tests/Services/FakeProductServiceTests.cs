using System;
using System.Linq;
using System.Threading.Tasks;
using SmartInventory.Core.Entities;
using SmartInventory.Core.Interfaces;
using Xunit;

namespace SmartInventory.Tests.Services
{
    /// <summary>
    /// Unit tests for the FakeProductService.
    /// Verifies that CRUD, pagination, filtering, and top-N behaviors work correctly.
    /// </summary>
    public class FakeProductServiceTests
    {
        /// <summary>
        /// Helper to create a fresh FakeProductService instance.
        /// Each instance is seeded with deterministic categories and products.
        /// </summary>
        private IProductService GetService() => new SmartInventory.Services.FakeProductService();

        // ----------------------- CREATE TESTS -----------------------

        [Fact]
        public async Task CreateAsync_ShouldAddProduct()
        {
            var service = GetService();

            var category = new Category { Id = Guid.NewGuid(), Name = "NewCategory" };
            var product = new Product { Name = "TestProduct", Quantity = 5, Category = category };

            var created = await service.CreateAsync(product);

            Assert.NotNull(created);
            Assert.Equal("TestProduct", created.Name);
            Assert.Equal(category.Id, created.CategoryId);
        }

        // ----------------------- READ TESTS -----------------------

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            var service = GetService();
            var all = await service.GetAllAsync();

            // Fake service seeds 10 products
            Assert.Equal(10, all.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            var service = GetService();
            var existing = (await service.GetAllAsync()).First();

            var fetched = await service.GetByIdAsync(existing.Id);

            Assert.NotNull(fetched);
            Assert.Equal(existing.Name, fetched!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var service = GetService();

            var result = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        // ----------------------- UPDATE TESTS -----------------------

        [Fact]
        public async Task UpdateAsync_ShouldModifyProduct()
        {
            var service = GetService();
            var product = (await service.GetAllAsync()).First();
            product.Name = "UpdatedName";

            await service.UpdateAsync(product);

            var updated = await service.GetByIdAsync(product.Id);
            Assert.Equal("UpdatedName", updated!.Name);
        }

        // ----------------------- DELETE TESTS -----------------------

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct_WhenExists()
        {
            var service = GetService();
            var product = (await service.GetAllAsync()).First();

            await service.DeleteAsync(product.Id);

            var remaining = await service.GetAllAsync();
            Assert.DoesNotContain(remaining, p => p.Id == product.Id);
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotFail_WhenProductDoesNotExist()
        {
            var service = GetService();
            await service.DeleteAsync(Guid.NewGuid());

            var all = await service.GetAllAsync();
            Assert.Equal(10, all.Count()); // seeded products remain
        }

        // ----------------------- PAGINATION / FILTERING -----------------------

        [Fact]
        public async Task GetPagedAsync_ShouldReturnCorrectPage()
        {
            var service = GetService();

            var (pageItems, totalCount) = await service.GetPagedAsync(2, 3);

            Assert.Equal(10, totalCount);
            Assert.Equal(3, pageItems.Count());
            Assert.Equal("Product04", pageItems.First().Name); // deterministic order
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilterByCategory()
        {
            var service = GetService();
            var catId = (await service.GetAllAsync()).First().CategoryId;

            var (products, _) = await service.GetPagedAsync(1, 10, categoryId: catId);

            Assert.All(products, p => Assert.Equal(catId, p.CategoryId));
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilterBySearchTerm()
        {
            var service = GetService();

            var (products, _) = await service.GetPagedAsync(1, 10, search: "Bar");

            Assert.All(products, p => Assert.Contains("Bar", p.Name + p.Description));
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilterByCategoryAndSearch()
        {
            var service = GetService();
            var catId = (await service.GetAllAsync()).First().CategoryId;

            var (products, _) = await service.GetPagedAsync(1, 10, categoryId: catId, search: "Bar");

            Assert.All(products, p =>
            {
                Assert.Equal(catId, p.CategoryId);
                Assert.Contains("Bar", p.Name + p.Description);
            });
        }

        // ----------------------- TOP-N TESTS -----------------------

        [Fact]
        public async Task GetTopAsync_ShouldReturnTopNProducts()
        {
            var service = GetService();

            var top = await service.GetTopAsync(3);

            Assert.Equal(3, top.Count());
            Assert.True(top.First().Quantity >= top.Last().Quantity);
        }

        [Fact]
        public async Task GetTopAsync_ShouldReturnAllIfCountExceedsTotal()
        {
            var service = GetService();

            var top = await service.GetTopAsync(20);

            Assert.Equal(10, top.Count());
        }
    }
}