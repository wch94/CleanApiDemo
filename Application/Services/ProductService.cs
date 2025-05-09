using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, IMapper mapper, ILogger<ProductService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(
        string? search,
        string sortBy,
        bool desc,
        int page,
        int pageSize)
    {
        var products = await _repository.GetAllAsync(search, sortBy, desc, page, pageSize);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> AddAsync(ProductDto dto)
    {
        _logger.LogInformation("Adding new product: {ProductName}", dto.Name);
        var product = _mapper.Map<Product>(dto);
        await _repository.AddAsync(product);
        _logger.LogInformation("Product added with id {ProductId}", product.Id);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> UpdateAsync(int id, ProductDto dto)
    {
        _logger.LogInformation("Updating product id {Id}", id);
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Update failed: product id {Id} not found", id);
            return false;
        }
        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing);
        _logger.LogInformation("Product id {Id} updated", id);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product id {Id}", id);
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Delete failed: product id {Id} not found", id);
            return false;
        }
        await _repository.DeleteAsync(product);
        _logger.LogInformation("Product id {Id} deleted", id);
        return true;
    }
}
