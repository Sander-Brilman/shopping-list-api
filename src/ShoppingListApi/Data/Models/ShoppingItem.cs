using System.ComponentModel.DataAnnotations;

namespace ShoppingListApi.Data.Models;

public sealed class ShoppingItemEntity
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int TimesUsed { get; set; }
    
    public bool IsChecked { get; set; } = false;

    public ShoppingListEntity List { get; set; }

    public Guid ListId { get; set; }
}
