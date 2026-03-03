namespace SmartInventory.Core.Interfaces;

using SmartInventory.Core.Entities;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);

    // New pagination method
    Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
            int page, int pageSize, Guid? categoryId = null, string? search = null);
}