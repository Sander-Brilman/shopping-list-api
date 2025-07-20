namespace Shared.DTO;

public sealed class ShoppingListResponse
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required int NumberOfItems { get; set; }

}
