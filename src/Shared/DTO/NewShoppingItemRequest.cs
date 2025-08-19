namespace Shared.DTO;

public sealed class NewShoppingItemRequest
{
    public Guid ListId { get; set; }

    public string Name { get; set; } = string.Empty;
}
