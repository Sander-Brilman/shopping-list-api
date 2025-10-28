using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MixPlannerApi.Data;

namespace MixPlannerApi.TaskList.Endpoints;

public static class GetItemsEndpoint
{
    public static readonly Func<AppDbContext, int, IAsyncEnumerable<TaskItemResponse>> GetTaskItemsByListId = EF.CompileAsyncQuery((AppDbContext dbContext, int listId) =>
        dbContext.Items
            .Where(i => i.ListId == listId)
            .Select(i => new TaskItemResponse
            {
                Name = i.Name
            }));

    public static async Task<List<TaskItemResponse>> GetAllItems(int listId, AppDbContext dbContext, IMemoryCache cache)
    {
        List<TaskItemResponse>? result = await cache.GetOrCreateAsync(listId, async (cache) =>
        {
            List<TaskItemResponse> items = [];
            await foreach (var item in GetTaskItemsByListId(dbContext, listId))
            {
                items.Add(item);
            }
            return items;
        }, CacheConfig.options);

        if (result is null)
        {
            cache.Remove(listId);
        }

        return result ?? [];
    }

}

public sealed class TaskItemResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public bool IsCompleted { get; set; }

    public int TimesCompleted { get; set; }
}
