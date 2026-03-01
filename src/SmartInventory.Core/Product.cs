namespace SmartInventory.Core.Entities;

/// <summary>
/// Represents a product in inventory.
/// This is a pure domain model and should contain no infrastructure logic.
/// </summary>
public class Product
{
    // Unique identifier
    public Guid Id { get; set; } = Guid.NewGuid();
    // Product name    
    public string Name { get; set; } = string.Empty;

    // Unique stock keeping unit
    public string SKU { get; set; } = string.Empty;

    // Current stock quantity
    public int Quantity { get; set; }

    // Price per unit
    public decimal Price { get; set; }

    // Foreign key
    public Guid CategoryID { get; set; }

    // Navigation property
    public Category? Category { get; set; }
}