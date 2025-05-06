using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ProductService service, ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] bool desc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("GET all products called: search={Search}, sortBy={SortBy}, desc={Desc}, page={Page}, pageSize={PageSize}", search, sortBy, desc, page, pageSize);
        var result = await _service.GetAllAsync(search, sortBy, desc, page, pageSize);
        _logger.LogInformation("GET all products returned {Count} items", result.Count());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET product by id {Id}", id);
        var dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            _logger.LogWarning("Product id {Id} not found", id);
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductDto dto)
    {
        _logger.LogInformation("POST create product: {ProductName}", dto.Name);
        var created = await _service.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductDto dto)
    {
        _logger.LogInformation("PUT update product id {Id}", id);
        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            _logger.LogWarning("PUT update failed: product id {Id} not found", id);
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("DELETE product id {Id}", id);
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            _logger.LogWarning("DELETE failed: product id {Id} not found", id);
            return NotFound();
        }
        return NoContent();
    }
}
