using Microsoft.AspNetCore.SignalR;
using MixPlannerApi.Data.Models;
using MixPlannerApi.TaskList.Endpoints;

namespace MixPlannerApi.LiveUpdate;

public class LiveUpdateService(IHubContext<LiveUpdateHub> hub)
{
    private readonly IHubContext<LiveUpdateHub> hub = hub;

    public async Task NotifyListUpdated()
    {

    }

    public async Task NotifyListDeleted()
    {

    }

    public async Task NotifyListCreated(TaskListResponse newList)
    {
        await hub.Clients.All.SendAsync(HubMethods.OnListAdded, newList);

    }

    public async Task NotifyItemCreated(TaskItemResponse item, int listId, CancellationToken token)
    {
        await hub.Clients.All.SendAsync(HubMethods.OnItemAdded, item, listId, token); 
    }

    public async Task NotifyItemUpdated(TaskItemDBEntity item, CancellationToken token)
    {
        await hub.Clients.All.SendAsync(HubMethods.OnItemToggled, item, token);

    }

    public async Task NotifyItemDeleted(int itemId, CancellationToken token)
    {
        await hub.Clients.All.SendAsync(HubMethods.OnItemDeleted, itemId, token);
    }

}