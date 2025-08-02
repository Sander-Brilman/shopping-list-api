using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DTO;
using ShoppingListApi.Data.Models;
using ShoppingListApi.Data;
using Microsoft.Extensions.Caching.Memory;

namespace ShoppingListApi.ShoppingItem;

public static class ShoppingItemEndpoints
{
    public static void MapShoppingItemEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", CreateNew).WithOpenApi();
        app.MapPatch("/{id:guid}/set-state/", SetState).WithOpenApi();
        app.MapDelete("/{id:guid}", DeleteShoppingItem).WithOpenApi();
    }

    public static async Task<IResult> CreateNew(NewShoppingItemRequest newItem, AppDbContext dbContext, IHubContext<ShoppingItemHub> hubContext, IMemoryCache cache, CancellationToken token)
    {
        string parsedName = newItem.Name.Trim();
        if (parsedName.Length == 0) {
            return Results.BadRequest("empty name not allowed");
        }


        ShoppingItemEntity shoppingItem = new() 
        {
            ListId = newItem.ListId,
            Name = parsedName,
            Id = Guid.NewGuid(),
            IsChecked = true,
            TimesUsed = 0,
        };

        dbContext.Items.Add(shoppingItem);
        await dbContext.SaveChangesAsync(token);

        ShoppingItemResponse itemResponse = shoppingItem.Adapt<ShoppingItemResponse>();
        await hubContext.Clients.All.SendAsync(ItemHubMethods.OnItemAdded, itemResponse, newItem.ListId, token); 

        if (cache.TryGetValue(newItem.ListId, out List<ShoppingItemResponse>? value)) {
            if (value is null)
            {
                cache.Remove(newItem.ListId);
            }
            else
            {
                value.Add(itemResponse);
            }
        }

        return Results.Created("", itemResponse);
    }
    

    public static async Task<IResult> SetState(Guid id, SetShoppingItemStateRequest setItemStateRequest, AppDbContext dbContext, IMemoryCache cache, IHubContext<ShoppingItemHub> hubContext, CancellationToken cancellationToken)
    {
        ShoppingItemEntity? item = await dbContext.Items
            .AsTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (item is null || item.IsChecked == setItemStateRequest.IsChecked)
        {
            return Results.NoContent();
        }

        item.TimesUsed++;
        item.IsChecked = setItemStateRequest.IsChecked;

        await dbContext.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.All.SendAsync(ItemHubMethods.OnItemToggled, item.Id, item.IsChecked, cancellationToken);

        if (cache.TryGetValue(item.ListId, out List<ShoppingItemResponse>? value)) {
            if (value is null)
            {
                cache.Remove(item.ListId);
            }
            else
            {
                ShoppingItemResponse? cacheItem = value.FirstOrDefault(i => i.Id == item.Id);
                if (cacheItem != null)
                {
                    cacheItem.IsChecked = setItemStateRequest.IsChecked;
                }
            }
        }

        return Results.NoContent();
    }



    public static async Task<IResult> DeleteShoppingItem(Guid id, AppDbContext dbContext, IMemoryCache cache, IHubContext<ShoppingItemHub> hubContext, CancellationToken cancellationToken)
    {
        ShoppingItemEntity? item = await dbContext.Items
            .AsTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (item is null) {
            return Results.NoContent();
        }

        dbContext.Items.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.All.SendAsync(ItemHubMethods.OnItemDeleted, id, cancellationToken);

        cache.Remove(item.ListId);

        return Results.NoContent();
    }

}
