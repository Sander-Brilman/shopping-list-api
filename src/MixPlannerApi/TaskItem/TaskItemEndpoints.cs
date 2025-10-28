using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MixPlannerApi.Data.Models;
using MixPlannerApi.Data;
using Microsoft.Extensions.Caching.Memory;
using MixPlannerApi.LiveUpdate;
using MixPlannerApi.TaskItem.Endpoints;

namespace MixPlannerApi.TaskItem;

public static class TaskItemEndpoints
{
    public static void MapTaskItemEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", CreateTaskItemEndpoint.Handler).WithOpenApi();
        app.MapPatch("/{id:guid}/set-state/", SetItemStateEndpoint.Handler).WithOpenApi();
        app.MapDelete("/{id:guid}", DeleteTaskItemEndpoint.Handler).WithOpenApi();
    }

}
