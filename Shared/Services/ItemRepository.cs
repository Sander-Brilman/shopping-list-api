using Shared.DTO;
using System.Net.Http.Json;

namespace Shared.Services;

public sealed class ItemRepository(HttpClient httpClient)
{
    private readonly HttpClient _http = httpClient;

    public async Task<ShoppingItemResponse> CreateFromContent(string newItemContent, Guid listId)
    {
        NewShoppingItemRequest newItem = new()
        {
            ListId = listId,
            Name = newItemContent
        };

        return await Create(newItem);
    }

    public async Task<ShoppingItemResponse> Create(NewShoppingItemRequest newItem)
    {
        var response = await _http.PostAsJsonAsync("/api/item/", newItem);

        if (response.IsSuccessStatusCode is false)
        {
            throw new HttpRequestException("failed to create new item", null, response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<ShoppingItemResponse>() ?? throw new Exception("Api did not return an item, :skull:");
    }

    public async Task SetItemState(ShoppingItemResponse item, bool isChecked)
    {
        item.IsChecked = isChecked;
        item.TimesUsed++;

        var response = await _http.PatchAsJsonAsync($"/api/item/{item.Id}/set-state/", new SetShoppingItemStateRequest(isChecked));

        if (response.IsSuccessStatusCode is false)
        {
            throw new HttpRequestException("failed to toggle item", null, response.StatusCode);
        }
    }

    public async Task<List<ShoppingItemResponse>> GetAllInList(Guid listId)
    {
        return (await _http.GetFromJsonAsync<List<ShoppingItemResponse>>($"/api/list/{listId}/items/"))!;    
    }

    public async Task DeleteItemFromList(ShoppingItemResponse item)
    {
        await _http.DeleteAsync($"/api/item/{item.Id}");
    }
}
