using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(
        string? search,
        string sortBy,
        bool desc,
        int page,
        int pageSize);
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
