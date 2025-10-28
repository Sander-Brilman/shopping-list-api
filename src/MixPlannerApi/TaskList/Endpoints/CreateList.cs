using System.Data;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using MixPlannerApi.Data;
using MixPlannerApi.Data.Models;
using MixPlannerApi.LiveUpdate;

namespace MixPlannerApi.TaskList.Endpoints;


public static class CreateTaskListEndpoint
{
    public static async Task<IResult> Handler(RequestBody body, AppDbContext dbContext, IMemoryCache cache, LiveUpdateService liveUpdateService)
    {
        new BodyValidator().ValidateAndThrow(body);

        string parsedName = body.Name.Trim();
        if (parsedName.Length == 0)
        {
            return Results.BadRequest("empty name not allowed");
        }

        TaskListDBEntity newList = new()
        {
            Name = parsedName,
        };

        dbContext.Lists.Add(newList);
        await dbContext.SaveChangesAsync();

        await liveUpdateService.NotifyListCreated(newList);

        cache.Remove(CacheConfig.Keys.AllShoppingLists);

        return Results.Created($"/List/${newList.Id}/items", newList);
    }

    public sealed record RequestBody(
        string Name,
        string Description,
        string? Color,
        int? ParentListId
    );

    public sealed class BodyValidator : AbstractValidator<RequestBody>
    {
        public BodyValidator()
        {
            RuleFor(r => r.Name).NotNull().MaximumLength(100);
            RuleFor(r => r.Description).NotNull();
        }
    }
}

