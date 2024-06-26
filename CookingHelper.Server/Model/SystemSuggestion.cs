using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingHelper.Model;

public class SystemSuggestion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SystemSuggestionId { get; set; } = default!;

    [ForeignKey("FeedbackGroupId")]
    public int FeedbackGroupId { get; set; } = default!;
    public FeedbackGroup FeedbackGroup { get; set; } = default!;

    public ICollection<FeedbackPost> PostList { get; set; } = new List<FeedbackPost>();
}
