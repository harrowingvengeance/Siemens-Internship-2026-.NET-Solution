using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _service;
    private readonly ILogger<ItemController> _logger;

    public ItemController(IItemService service, ILogger<ItemController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("[LOG] {Time}: GET api/item called", DateTime.UtcNow);

        var items = await _service.GetAllAsync();
        var itemList = items.ToList();

        var totalCount = itemList.Count;
        var averageValue = itemList.Any() ? itemList.Average(i => i.Value) : 0;

        _logger.LogInformation("[LOG] Returning {Count} items, average value: {Average}", totalCount, averageValue);

        return Ok(new
        {
            Data = itemList,
            Statistics = new
            {
                TotalCount = totalCount,
                AverageValue = averageValue,
                RetrievedAt = DateTime.UtcNow
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("[LOG] {Time}: GET api/item/{Id} called", DateTime.UtcNow, id);

        if (id <= 0)
        {
            _logger.LogInformation("[LOG] Invalid id: {Id}", id);
            return BadRequest("Id must be a positive integer.");
        }

        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            _logger.LogInformation("[LOG] Item {Id} not found", id);
            return NotFound($"Item with Id {id} was not found.");
        }

        return Ok(item);
    }

    [HttpGet("passing/{n}")]
    public async Task<IActionResult> GetFirstNPassing(int n)
    {
        _logger.LogInformation("[LOG] {Time}: GET api/item/passing/{N} called", DateTime.UtcNow, n);

        if (n <= 0)
        {
            return BadRequest("N must be a positive integer.");
        }

        var items = await _service.GetFirstNPassingActiveAsync(n);
        return Ok(items);
    }
}
