using SmartInventory.Core.Entities;
using Xunit;

namespace SmartInventory.Core.Entities.Tests
{
    public class ProductTests
    {
        [Fact]
        public void DefaultConstructor_ShouldCreateProduct()
        {
            var product = new Product();

            Assert.NotNull(product);
        }

        [Fact]
        public void ShouldSetPropertiesCorrectly()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Quantity = 5
            };

            Assert.Equal("Test Product", product.Name);
            Assert.Equal(5, product.Quantity);
        }

        [Fact]
        public void NullableCategory_ShouldBeNullByDefault()
        {
            var product = new Product();

            Assert.Null(product.Category);
        }
    }
}