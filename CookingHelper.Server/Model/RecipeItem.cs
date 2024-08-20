using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RecipeItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RecipeItemId { get; set; } = default!;

    [ForeignKey("UserId")]
    public int UserId { get; set; } = default!;

    public string RecipeItemName { get; set; } = default!;

    public List<string> RecipeStep { get; set; } = new List<string>();

    public byte[] RecipeImageContent { get; set; } = default!;
}
