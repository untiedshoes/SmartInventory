using SmartInventory.Core.Entities;

namespace SmartInventory.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
            int page, int pageSize, Guid? categoryId = null, string? search = null);
        Task<IEnumerable<Product>> GetTopAsync(int count);
    }
}