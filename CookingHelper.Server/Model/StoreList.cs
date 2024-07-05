using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CookingHelper.Model;

public class StoreList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StoreListId { get; set; } = default!;

    [ForeignKey("UserId")]
    public string UserId { get; set; } = default!;
    public UserList UserList { get; set; } = default!;
    public ICollection<StoreItemGroup> StoreItemGroup { get; set; } = new List<StoreItemGroup>();
}
