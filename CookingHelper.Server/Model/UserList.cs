using System.ComponentModel.DataAnnotations;

namespace CookingHelper.Model;

public class UserList
{
    [Key]
    public string UserId { get; set; } = default!;
    public string? ShoppingListText { get; set; }

    public ICollection<StoreItem> StoreList { get; set; } = new List<StoreItem>();

    public ICollection<RecipeItem> RecipeList { get; set; } = new List<RecipeItem>();
}
