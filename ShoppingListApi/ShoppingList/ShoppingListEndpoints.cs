using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using ShoppingListApi.Data.Models;
using ShoppingListApi.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.SignalR;
using Shared;

namespace ShoppingListApi.ShoppingList;

public static class ShoppingListEndpoints
{
    public static void MapShoppingListEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetAll).WithOpenApi();
        app.MapGet("/{listId:guid}/items", GetAllItems).WithOpenApi();
        app.MapPost("/", CreateNew).WithOpenApi();
        app.MapDelete("/{id:guid}", Delete).WithOpenApi();
    }

    public static readonly Func<AppDbContext, Guid, IAsyncEnumerable<ShoppingItemResponse>> GetShoppingItemsByListId = EF.CompileAsyncQuery((AppDbContext dbContext, Guid listId) =>
        dbContext.Items
            .Where(i => i.ListId == listId)
            .Select(i => new ShoppingItemResponse
            {
                Id = i.Id,
                Name = i.Name,
                IsChecked = i.IsChecked,
                TimesUsed = i.TimesUsed
            }));


    public static readonly Func<AppDbContext, IAsyncEnumerable<ShoppingListResponse>> GetShoppingLists = EF.CompileAsyncQuery((AppDbContext dbContext) =>
        dbContext.Lists
            .OrderByDescending(l => l.Items.Count)
            .Select(l => new ShoppingListResponse
            {
                Id = l.Id,
                Name = l.Name,
                NumberOfItems = l.Items.Count,
            }));


    public static async Task<ShoppingListResponse[]> GetAll(AppDbContext dbContext, IMemoryCache cache)
    {
        ShoppingListResponse[]? result = await cache.GetOrCreateAsync<ShoppingListResponse[]>(CacheConfig.Keys.AllShoppingLists, async (cache) =>
        {
            List<ShoppingListResponse> lists = [];
            await foreach (var list in GetShoppingLists(dbContext))
            {
                lists.Add(list);
            }
            return [.. lists];
        }, CacheConfig.options);

        return result ?? [];
    }

    public static async Task<List<ShoppingItemResponse>> GetAllItems(Guid listId, AppDbContext dbContext, IMemoryCache cache)
    {
        List<ShoppingItemResponse>? result = await cache.GetOrCreateAsync(listId, async (cache) =>
        {
            List<ShoppingItemResponse> items = [];
            await foreach (var item in GetShoppingItemsByListId(dbContext, listId))
            {
                items.Add(item);
            }
            return items;
        }, CacheConfig.options);

        return result ?? [];
    }

    public static async Task<IResult> CreateNew(CreateShoppingListRequest body, AppDbContext dbContext, IMemoryCache cache, IHubContext<ShoppingListHub> hubContext)
    {
        string parsedName = body.Name.Trim();
        if (parsedName.Length == 0) {
            return Results.BadRequest("empty name not allowed");
        }

        ShoppingListEntity newList = new() {
            Name = parsedName,
        };

        dbContext.Lists.Add(newList);
        await dbContext.SaveChangesAsync();
        await hubContext.Clients.All.SendAsync(ListHubMethods.OnListAdded, newList.Name, newList.Id);

        cache.Remove(CacheConfig.Keys.AllShoppingLists);

        return Results.Created($"/List/${newList.Id}/items", newList);
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
}
