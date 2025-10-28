using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MixPlannerApi.Data;
using MixPlannerApi.Data.Models;
using MixPlannerApi.LiveUpdate;

namespace MixPlannerApi.TaskItem.Endpoints;

public static class DeleteTaskItemEndpoint
{
    
    public static async Task<IResult> Handler(int id, AppDbContext dbContext, IMemoryCache cache, LiveUpdateService liveUpdateService, CancellationToken token)
    {
        TaskItemDBEntity? item = await dbContext.Items
            .AsTracking()
            .FirstOrDefaultAsync(i => i.Id == id, token);

        if (item is null) {
            return Results.NoContent();
        }

        dbContext.Items.Remove(item);
        await dbContext.SaveChangesAsync(token);
        await liveUpdateService.NotifyItemDeleted(id, token);

        cache.Remove(item.ListId);

        return Results.NoContent();
    }
}