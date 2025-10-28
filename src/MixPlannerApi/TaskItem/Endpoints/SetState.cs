using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MixPlannerApi.Data;
using MixPlannerApi.Data.Models;
using MixPlannerApi.LiveUpdate;
using MixPlannerApi.TaskList.Endpoints;

namespace MixPlannerApi.TaskItem.Endpoints;

public static class SetItemStateEndpoint
{
    public static async Task<IResult> Handler(int id, RequestBody request, AppDbContext dbContext, IMemoryCache cache, LiveUpdateService liveUpdateService, CancellationToken token)
    {
        TaskItemDBEntity? item = await dbContext.Items
            .AsTracking()
            .FirstOrDefaultAsync(i => i.Id == id, token);

        if (item is null || item.IsCompleted == request.IsCompleted)
        {
            return Results.NoContent();
        }

        if (request.IsCompleted)
        {
            item.TimesCompleted++;
        }

        item.IsCompleted = request.IsCompleted;

        await dbContext.SaveChangesAsync(token);

        await liveUpdateService.NotifyItemUpdated(item, token);


        if (cache.TryGetValue(item.ListId, out List<TaskItemResponse>? cachedItems))
        {
            if (cachedItems is null)// if we get null something went wrong setting the data, remove the invalid data
            {
                cache.Remove(item.ListId);
                return Results.NoContent();
            }

            TaskItemResponse? cachedItem = cachedItems.FirstOrDefault(i => i.Id == item.Id);

            if (cachedItem is null)// if the cached item is null here that means we have a outdated cache. remove it. 
            {
                cache.Remove(item.ListId);
                return Results.NoContent();
            }

            cachedItem.IsCompleted = request.IsCompleted;
        }

        return Results.NoContent();
    }
    
    public sealed record RequestBody(bool IsCompleted);
}

