using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public class ItemService : IItemService
{
    private readonly IItemReader _repository;

    public ItemService(IItemReader repository)
    {
        _repository = repository;
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item is { IsActive: true } ? item : null;
    }

    public async Task<IEnumerable<Item>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Where(i => i.IsActive);
    }

    
    // returns the given N active grades that are equal to or above 5 using LINQ methods (where and take)
    public async Task<IEnumerable<Item>> GetFirstNPassingActiveAsync(int n)
    {
        var items = await _repository.GetAllAsync();
        return items
            .Where(i => i.IsActive && i.Value >= 5)
            .Take(n);
    }
}
