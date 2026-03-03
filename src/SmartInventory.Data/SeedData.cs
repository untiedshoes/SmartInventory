using SmartInventory.Core.Entities;

namespace SmartInventory.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(InventoryDbContext context)
        {
            // -----------------------------
            // Remove existing data (tables already exist)
            // -----------------------------
            context.Products.RemoveRange(context.Products);
            context.Categories.RemoveRange(context.Categories);
            await context.SaveChangesAsync();

            // -----------------------------
            // Seed 15 categories
            // -----------------------------
            var categories = new List<Category>();
            for (int i = 1; i <= 15; i++)
            {
                categories.Add(new Category
                {
                    Name = $"Category {i}",
                    Description = $"Description for Category {i}"
                });
            }
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // -----------------------------
            // Seed 10 products per category = 150 products
            // -----------------------------
            var products = new List<Product>();
            var random = new Random();

            foreach (var category in categories)
            {
                for (int i = 1; i <= 10; i++)
                {
                    products.Add(new Product
                    {
                        Name = $"{category.Name} - Product {i}",
                        Description = $"Description for {category.Name} Product {i}",
                        Price = Math.Round((decimal)(random.NextDouble() * 500 + 10), 2),
                        Quantity = random.Next(0, 200),
                        CategoryId = category.Id
                    });
                }
            }

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Seeded {categories.Count} categories and {products.Count} products.");
        }
    }
}