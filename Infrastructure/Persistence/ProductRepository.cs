using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(
        string? search,
        string sortBy,
        bool desc,
        int page,
        int pageSize)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        // Sorting
        query = sortBy.ToLower() switch
        {
            "name" => desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            _ => query.OrderBy(p => p.Name) // default fallback
        };

        // Paging
        query = query.Skip((page - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
