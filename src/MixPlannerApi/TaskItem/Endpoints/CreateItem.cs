using Mapster;
using Microsoft.Extensions.Caching.Memory;
using MixPlannerApi.Data;
using MixPlannerApi.Data.Models;
using MixPlannerApi.LiveUpdate;
using MixPlannerApi.TaskList.Endpoints;

namespace MixPlannerApi.TaskItem.Endpoints;

public static class CreateTaskItemEndpoint
{
    public static async Task<IResult> Handler(RequestBody request, AppDbContext dbContext, LiveUpdateService liveUpdateService, IMemoryCache cache, CancellationToken token)
    {
        string parsedName = request.Name.Trim();
        if (parsedName.Length == 0) {
            return Results.BadRequest("empty name not allowed");
        }


        TaskItemDBEntity newItem = new() 
        {
            ListId = request.ListId,
            Name = parsedName,
        };

        dbContext.Items.Add(newItem);
        await dbContext.SaveChangesAsync(token);

        TaskItemResponse response = newItem.Adapt<TaskItemResponse>();

        await liveUpdateService.NotifyItemCreated(response, request.ListId, token);

        if (cache.TryGetValue(request.ListId, out List<TaskItemResponse>? cachedItems)) {
            if (cachedItems is null)
            {
                cache.Remove(request.ListId);
            }
            else
            {
                cachedItems.Add(response);
            }
        }

        return Results.Created("", response);
    }
    

    public sealed record RequestBody(int ListId, string Name);
}


