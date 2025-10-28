using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MixPlannerApi.Data;

namespace MixPlannerApi.TaskList.Endpoints;

public static class GetAllTaskListsEndpoint
{
    public static readonly Func<AppDbContext, IAsyncEnumerable<ShoppingListResponse>> GetShoppingListsQuery = EF.CompileAsyncQuery((AppDbContext dbContext) =>
        dbContext.Lists
            .OrderByDescending(l => l.Items.Count)
            .Select(l => new ShoppingListResponse
            {
                Id = l.Id,
                Name = l.Name,
                NumberOfItems = l.Items.Count,
            }));


    public static async Task<ShoppingListResponse[]> Handler(AppDbContext dbContext, IMemoryCache cache)
    {
        ShoppingListResponse[]? result = await cache.GetOrCreateAsync<ShoppingListResponse[]>(CacheConfig.Keys.AllShoppingLists, async (cache) =>
        {
            List<ShoppingListResponse> lists = [];
            await foreach (var list in GetShoppingListsQuery(dbContext))
            {
                lists.Add(list);
            }
            return [.. lists];
        }, CacheConfig.options);

        if (result is null) // remove faulty cache entry
        {
            cache.Remove(CacheConfig.Keys.AllShoppingLists);
        }

        return result ?? [];
    }

}

public sealed class ShoppingListResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required int NumberOfItems { get; set; }
}
