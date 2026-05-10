using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IItemService
{
    Task<Item?> GetByIdAsync(int id);
    Task<IEnumerable<Item>> GetAllAsync();
    Task<IEnumerable<Item>> GetFirstNPassingActiveAsync(int n);
}
