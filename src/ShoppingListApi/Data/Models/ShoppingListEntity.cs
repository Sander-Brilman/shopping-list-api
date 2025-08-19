using System.ComponentModel.DataAnnotations;

namespace ShoppingListApi.Data.Models;

public sealed class ShoppingListEntity
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public List<ShoppingItemEntity> Items { get; set; } = [];
}
