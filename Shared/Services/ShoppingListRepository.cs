using System;
using System.Net.Http.Json;
using Shared.DTO;


namespace Shared.Services;

public sealed class ShoppingListRepository(HttpClient http)
{
    private readonly HttpClient http = http;

    private ShoppingListResponse[]? lists;
    public async Task<ShoppingListResponse[]> GetAllLists()
    {
        lists ??= ((await http.GetFromJsonAsync<ShoppingListResponse[]>("/api/List/")) ?? [])
            .OrderByDescending(l => l.NumberOfItems)
            .ToArray();
        return lists;
    }
}
