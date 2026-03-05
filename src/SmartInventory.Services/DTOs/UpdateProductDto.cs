namespace SmartInventory.Services.DTOs
{
    /// <summary>
    /// DTO for updating an existing product.
    /// Includes Id so API knows which product to update.
    /// </summary>
    public class UpdateProductDto
    {
        /// <summary>
        /// Unique identifier of the product to update.
        /// </summary>
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
    }
}