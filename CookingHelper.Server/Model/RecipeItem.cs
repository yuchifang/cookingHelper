using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingHelper.Model;

public class RecipeItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RecipeItemId { get; set; } = default!;

    [ForeignKey("UserId")]
    public string UserId { get; set; } = default!;
    public UserList UserList { get; set; } = default!;

    public string Name { get; set; } = default!;

    public List<string> Step { get; set; } = new List<string>();

    public byte[]? ImageContent { get; set; } = default!;

    public string Ingredients { get; set; } = default!;
}
