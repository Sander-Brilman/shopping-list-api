using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MixPlannerApi.Data;
using MixPlannerApi.TaskItem;

namespace MixPlannerApi.Health;

public static class HealthEndpoints
{
    public static void MapHealthEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", HealthEndpoint).WithOpenApi();
    }

    public static async Task<IResult> HealthEndpoint(AppDbContext dbContext)
    {
        try
        {
            await dbContext.Lists.FirstAsync();
        }
        catch (Exception)
        {
            return Results.InternalServerError("Failed to connect to DB");
        }

        return Results.Ok();
    }

    
}