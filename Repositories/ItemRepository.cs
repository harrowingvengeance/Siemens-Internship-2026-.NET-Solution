using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Siemens.Internship2026.GradeBook.Repositories;

public class ItemRepository : IItemReader
{
    private const string ExternalEndpoint =
        "https://gist.githubusercontent.com/ArdeleanTudor/8ea407832cd9794960e0e6bbd1319f6e/raw/145b121103dd1cee3737a681c487f7295ac82e6b/gistfile1.txt";

    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ItemRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        var items = await FetchItemsAsync();
        return items.FirstOrDefault(i => i.Id == id);
    }

    public async Task<IEnumerable<Item>> GetAllAsync()
    {
        return await FetchItemsAsync();
    }

    private async Task<List<Item>> FetchItemsAsync()
    {
        var response = await _httpClient.GetAsync(ExternalEndpoint);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var wrapper = JsonSerializer.Deserialize<ItemWrapper>(json, JsonOptions);
        return wrapper?.Items ?? new List<Item>();
    }

    private sealed class ItemWrapper
    {
        public List<Item> Items { get; set; } = new();
    }
}
