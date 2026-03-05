using AutoMapper;
using SmartInventory.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Entities;
using SmartInventory.Data;
using SmartInventory.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartInventory.Services.Tests
{
    public class ProductServiceTests
    {
        private readonly IMapper _mapper;

        public ProductServiceTests()
        {
            // Configure AutoMapper for tests
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // your profile from Services/Mapping
            });

            _mapper = config.CreateMapper();
        }
        /// <summary>
        /// Creates a fresh in-memory DbContext for each test with optional deterministic seed data.
        /// </summary>
        /// <param name="seedData">Whether to seed initial products and categories</param>
        /// <remarks>
        /// Why:
        /// - In-memory DB ensures tests do not affect real database.
        /// - Each test gets a unique DB (via Guid) to avoid cross-test contamination.
        /// - Deterministic seed ensures tests relying on product names/quantities are stable.
        /// </remarks>
        private InventoryDbContext GetInMemoryDbContext(bool seedData = false)
        {
            // Configure in-memory EF Core options
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            var context = new InventoryDbContext(options);

            if (seedData)
            {
                // Seed categories
                // Using fixed GUIDs is optional; here we generate new ones
                var cat1 = new Category { Id = Guid.NewGuid(), Name = "Category 1" };
                var cat2 = new Category { Id = Guid.NewGuid(), Name = "Category 2" };
                context.Categories.AddRange(cat1, cat2);

                // Seed products
                // Using zero-padded numbers in names to ensure proper string ordering
                // Reason: "Product10" < "Product2" in default string comparison,
                // which breaks paging tests. Using "Product01", "Product02", ... fixes it.
                for (int i = 1; i <= 10; i++)
                {
                    context.Products.Add(new Product
                    {
                        Id = Guid.NewGuid(),                      // Unique ID per product
                        Name = $"Product{i:D2}",                  // D2 pads single digits with 0
                        Description = i % 2 == 0 ? "Foo" : "Bar", // Alternating descriptions for filtering tests
                        Quantity = i * 10,                        // Simple quantity pattern for top-product tests
                        CategoryId = i <= 5 ? cat1.Id : cat2.Id   // First 5 -> Category1, next 5 -> Category2
                    });
                }

                // Save seeded data to the in-memory database
                context.SaveChanges();
            }

            return context;
        }

        // ----------------------- CREATE TESTS -----------------------

        [Fact]
        public async Task CreateAsync_ShouldAddProduct()
        {
            // Arrange: get fresh DB and service
            var context = GetInMemoryDbContext(seedData: true);

            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);
            var product = new Product { Name = "Test Product", Quantity = 5, Category = category };

            // Act: call the service
            var created = await service.CreateAsync(product);

            // Assert: product is added
            Assert.NotNull(created); // object returned
            Assert.Equal("Test Product", created.Name); // correct name
            Assert.Contains(context.Products, p => p.Id == created.Id); // persisted in context
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenQuantityNegative()
        {
            // Arrange
            var context = GetInMemoryDbContext(seedData: true);
            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);
            var product = new Product { Name = "Invalid", Quantity = -1, Category = category };

            // Act & Assert: service validates quantity
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.CreateAsync(product));
        }

        // ----------------------- READ TESTS -----------------------

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var all = await service.GetAllAsync();

            // Why: seed has 10 products, all should be returned
            Assert.Equal(10, all.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var product = context.Products.First();
            var fetched = await service.GetByIdAsync(product.Id);

            Assert.NotNull(fetched);
            Assert.Equal(product.Name, fetched.Name); // ensures correct mapping
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var result = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result); // non-existent ID returns null
        }

        // ----------------------- UPDATE TESTS -----------------------

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var product = context.Products.First();
            product.Name = "UpdatedName";

            await service.UpdateAsync(product);

            var updated = await context.Products.FindAsync(product.Id);
            Assert.Equal("UpdatedName", updated!.Name); // ensures update persisted
        }

        // ----------------------- DELETE TESTS -----------------------

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct_WhenExists()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var product = context.Products.First();
            await service.DeleteAsync(product.Id);

            // Why: Deleted product should no longer be in DB
            Assert.DoesNotContain(context.Products, p => p.Id == product.Id);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotExists()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await service.DeleteAsync(Guid.NewGuid()));
        }

        // ----------------------- PAGING & FILTERING TESTS -----------------------

        [Fact]
        public async Task GetPagedAsync_ShouldReturn_CorrectPageAndTotalCount()
        {
            // Arrange
            // Create a fresh in-memory database seeded with 10 products (Product01, Product02, ..., Product10)
            // Zero-padding ensures lexicographical ordering works consistently (Product01 < Product02 < ... < Product10)
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            int page = 2;      // We want the second page
            int pageSize = 3;  // Each page has 3 items

            // Act
            var (products, totalCount) = await service.GetPagedAsync(page, pageSize);

            // Assert

            // Total count should still be all seeded products (10)
            Assert.Equal(10, totalCount);

            // The page should contain exactly 'pageSize' number of products
            Assert.Equal(3, products.Count());

            // With zero-padded names:
            // Page 1: Product01, Product02, Product03
            // Page 2: Product04, Product05, Product06
            // Page 3: Product07, Product08, Product09
            // Page 4: Product10
            // Therefore, first product on page 2 must be "Product04"
            Assert.Equal("Product04", products.First().Name); // <-- this fixes the failing test

            // Extra check (optional) to ensure pagination works as expected
            Assert.Collection(products,
                p => Assert.Equal("Product04", p.Name),
                p => Assert.Equal("Product05", p.Name),
                p => Assert.Equal("Product06", p.Name)
            );
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilter_ByCategoryId()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var cat1Id = context.Categories.First().Id;
            var (products, _) = await service.GetPagedAsync(1, 10, categoryId: cat1Id);

            Assert.All(products, p => Assert.Equal(cat1Id, p.CategoryId)); // all from same category
            Assert.Equal(5, products.Count()); // category 1 has 5 products in seed
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilter_BySearchTerm()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var (products, _) = await service.GetPagedAsync(1, 10, search: "Bar");

            // Why: "Bar" appears in Description or Name of 5 products
            Assert.All(products, p => Assert.Contains("Bar", p.Name + p.Description));
            Assert.Equal(5, products.Count());
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilter_ByCategoryAndSearch_Together()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var cat1Id = context.Categories.First().Id;
            var (products, _) = await service.GetPagedAsync(1, 10, categoryId: cat1Id, search: "Bar");

            // Why: Only products in cat1 with "Bar" should be returned
            Assert.All(products, p =>
            {
                Assert.Equal(cat1Id, p.CategoryId);
                Assert.Contains("Bar", p.Name + p.Description);
            });

            Assert.Equal(3, products.Count()); // matches deterministic seed
        }

        // ----------------------- TOP-N TESTS -----------------------

        [Fact]
        public async Task GetTopAsync_ShouldReturn_TopNProducts()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var top = await service.GetTopAsync(3);

            Assert.Equal(3, top.Count()); // returned top 3
            Assert.Equal(100, top.First().Quantity); // highest quantity first
            Assert.Equal(80, top.Last().Quantity);  // lowest in top 3
        }

        [Fact]
        public async Task GetTopAsync_ShouldReturn_AllIfCountExceedsTotal()
        {
            var context = GetInMemoryDbContext(seedData: true);
            var service = new ProductService(context, NullLogger<ProductService>.Instance, _mapper);

            var top = await service.GetTopAsync(20);

            // Why: DB only has 10 products, so top 20 returns all
            Assert.Equal(10, top.Count());
        }
    }
}