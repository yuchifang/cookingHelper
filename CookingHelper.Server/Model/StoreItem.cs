using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CookingHelper.LineDtoService;

namespace CookingHelper.Model;

public class StoreItem : StorageInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StoreItemId { get; set; } = default!;

    [ForeignKey("StoreListId")]
    public int StoreListId { get; set; } = default!;
    public StoreList StoreList { get; set; } = default!;
}
