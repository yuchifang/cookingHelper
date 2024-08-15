using System.ComponentModel.DataAnnotations;

namespace CookingHelper.Model;

public class UserList
{
    [Key]
    public string UserId { get; set; } = default!;
    public string? ShoppingListText { get; set; }

    public virtual StoreList? StoreList { get; set; }
}
