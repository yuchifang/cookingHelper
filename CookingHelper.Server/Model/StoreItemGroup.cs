using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class StoreItemGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StoreItemGroupId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Place { get; set; } = default!;
    public string? Location { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    [ForeignKey("StoreItemListId")]
    public int StoreItemListId { get; set; } = default!;
    public StoreItemList StoreItemList { get; set; } = default!;
}
