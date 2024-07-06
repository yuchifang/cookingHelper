using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingHelper.Model;

public class StoreItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StoreItemGroupId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Place { get; set; } = default!;
    public string? Location { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    [ForeignKey("StoreListId")]
    public int StoreListId { get; set; } = default!;
    public StoreList StoreList { get; set; } = default!;
}
