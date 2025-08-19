namespace ShoppingListUIBlazor.Components.Shared.AlertBox;

public readonly record struct AlertBoxContent(
    string AlertText, 
    string ButtonText, 
    string BootstrapTheme,
    Func<Task> OnButtonClick);
