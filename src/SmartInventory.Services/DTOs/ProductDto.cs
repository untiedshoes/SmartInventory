namespace SmartInventory.Services.DTOs
{
    /// <summary>
    /// Data Transfer Object for sending Product data to clients.
    /// Keeps internal entity details hidden and allows for future API versioning.
    /// </summary>
    public class ProductDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = "";
        public string? Description { get; init; }
        public int Quantity { get; init; }
        public Guid CategoryId { get; init; }

        // Expose Category name for frontend display without exposing full Category entity
        public string? CategoryName { get; init; }
    }
}