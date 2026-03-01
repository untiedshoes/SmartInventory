using Microsoft.EntityFrameworkCore;
using SmartInventory.Data;
using SmartInventory.Core.Entities;
using Xunit;

namespace SmartInventory.Data.Tests
{
    public class InventoryDbContextTests
    {
        private InventoryDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new InventoryDbContext(options);
        }

        [Fact]
        public void CanAddProduct()
        {
            using var context = GetInMemoryDbContext();

            var category = new Category { Name = "TestCategory" };
            context.Categories.Add(category);
            context.SaveChanges();

            context.Products.Add(new Product { Name = "Test Product", Quantity = 3, Category = category });
            context.SaveChanges();

            Assert.Equal(1, context.Products.Count());
        }

        [Fact]
        public void CanAddCategory()
        {
            using var context = GetInMemoryDbContext();

            var category = new Category { Name = "Category1" };
            context.Categories.Add(category);
            context.SaveChanges();

            Assert.Equal(1, context.Categories.Count());
        }

        [Fact]
        public void Product_HasCategoryRelationship()
        {
            using var context = GetInMemoryDbContext();

            var category = new Category { Name = "Cat" };
            context.Categories.Add(category);
            context.SaveChanges();

            var product = new Product { Name = "P1", Quantity = 1, Category = category };
            context.Products.Add(product);
            context.SaveChanges();

            var fetched = context.Products.Include(p => p.Category).FirstOrDefault();
            Assert.NotNull(fetched);
            Assert.NotNull(fetched.Category);
            Assert.Equal("Cat", fetched.Category.Name);
        }
    }
}