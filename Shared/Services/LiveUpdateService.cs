using Microsoft.AspNetCore.SignalR.Client;
using Shared.DTO;

namespace Shared.Services;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public sealed class LiveUpdateService : IAsyncDisposable
{
    private HubConnection? hub;

    public event Func<ShoppingItemResponse, Guid, Task> OnItemAdded;
    public event Func<Guid, bool, Task> OnItemToggled;
    public event Func<Guid, Task> OnItemDeleted;
    public event Func<Task> OnReconnecting;

    public async Task ConnectTo(string apiUrl) 
    {
        if (hub is not null && hub.State == HubConnectionState.Connected)
        {
            return;
        }

        hub = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl($"{apiUrl}/ShoppingItemHub")
            .Build();

        await hub.StopAsync();

        hub.On(ItemHubMethods.OnItemAdded, async (ShoppingItemResponse item, Guid listId) =>
        {
            await OnItemAdded.Invoke(item, listId);
        });

        hub.On(ItemHubMethods.OnItemToggled, async (Guid id, bool isChecked) =>
        {
            await OnItemToggled.Invoke(id, isChecked);
        });

        hub.On(ItemHubMethods.OnItemDeleted, async (Guid id) =>
        {
            await OnItemDeleted.Invoke(id);
        });

        hub.Reconnecting += async (_) =>
        {
            await OnReconnecting.Invoke();
        };

        await hub.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (hub is null) {
            return;
        }

        await hub.StopAsync();
        await hub.DisposeAsync();
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
