using Core.Entities;
using Core.Interfaces;

namespace Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return _repository.GetAllAsync();
    }
}
