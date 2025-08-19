using Shared.DTO;

namespace ShoppingListUIBlazor;

public static class ListExtentions
{
    public static List<ShoppingItemResponse> Search(this List<ShoppingItemResponse> orginalList, string searchQuery)
    {
        searchQuery = searchQuery.Trim().ToLower();

        return 
        [.. 
            orginalList
                .Where(i => i.Name.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase))
                    .OrderBy(i => i.IsChecked)
                .ThenByDescending(i => i.Name
                    .StartsWith(searchQuery, StringComparison.CurrentCultureIgnoreCase)
                        ? (i.Name.Equals(searchQuery, StringComparison.CurrentCultureIgnoreCase) ? 2 : 1)
                        : 0
                )
        ];
    }
}
