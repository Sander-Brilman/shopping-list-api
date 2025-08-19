using ShoppingListUIBlazor.Components.Shared.AlertBox;

namespace ShoppingListUIBlazor.Services;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public sealed class AlertBoxService
{
    public event Func<AlertBoxContent, Task> OnContentSet;

    public async Task SetContentAsync(AlertBoxContent alertBoxContent)
    {
        await OnContentSet.Invoke(alertBoxContent);
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.