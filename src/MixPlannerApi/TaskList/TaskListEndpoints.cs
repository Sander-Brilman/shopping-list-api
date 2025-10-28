using Microsoft.EntityFrameworkCore;
using MixPlannerApi.Data.Models;
using MixPlannerApi.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.SignalR;
using MixPlannerApi.TaskList.Endpoints;

namespace MixPlannerApi.TaskList;

public static class TaskListEndpoints
{
    public static void MapTaskListEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetAllTaskListsEndpoint.Handler).WithOpenApi();
        app.MapGet("/{listId:guid}/items", GetAllItems).WithOpenApi();
        app.MapGet("/clear-cache", ClearCache).WithOpenApi();
        app.MapPost("/", CreateNew).WithOpenApi();
        app.MapDelete("/{id:guid}", Delete).WithOpenApi();
    }


    public static async Task<IResult> Delete(Guid id, AppDbContext dbContext, IMemoryCache cache, IHubContext<ShoppingListHub> hubContext)
    {
        cache.Remove(CacheConfig.Keys.AllShoppingLists);

        await dbContext.Lists
            .Where(l => l.Id == id)
            .ExecuteDeleteAsync();
        await hubContext.Clients.All.SendAsync(ListHubMethods.OnListDeleted, id);

        return Results.NoContent();
    }

    public static IResult ClearCache(IMemoryCache cache)
    {
        cache.Remove(CacheConfig.Keys.AllShoppingLists);
        return Results.NoContent();
    }
}
