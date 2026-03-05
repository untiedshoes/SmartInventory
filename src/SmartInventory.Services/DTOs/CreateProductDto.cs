namespace SmartInventory.Services.DTOs
{
    /// <summary>
    /// Used when creating a new product via API.
    /// Allows validation and shields internal IDs/fields from clients.
    /// </summary>
    public class CreateProductDto
    {
        public string Name { get; init; } = "";
        public string? Description { get; init; }
        public int Quantity { get; init; }
        public Guid CategoryId { get; init; }
    }
}