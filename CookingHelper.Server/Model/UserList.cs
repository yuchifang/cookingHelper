using System.ComponentModel.DataAnnotations;

public class UserList
{
    [Key]
    public string UserId { get; set; } = default!;
    public string? ShoppingListText { get; set; }
}