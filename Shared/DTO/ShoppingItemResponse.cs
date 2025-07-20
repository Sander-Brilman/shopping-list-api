namespace Shared.DTO;

public sealed class ShoppingItemResponse
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public bool IsChecked { get; set; }

    public int TimesUsed { get; set; }
}
