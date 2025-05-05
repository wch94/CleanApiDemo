using Application.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> AddAsync(ProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        await _repository.AddAsync(product);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> UpdateAsync(int id, ProductDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        _mapper.Map(dto, existing); // map updates into entity
        await _repository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return false;
        await _repository.DeleteAsync(product);
        return true;
    }
}
