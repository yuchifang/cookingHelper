using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingHelper.Model;

public class FeedbackGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FeedbackGroupId { get; set; }
    public QuestionReply QuestionReply { get; set; } = default!;
    public SystemSuggestion SystemSuggestion { get; set; } = default!;

    // public OtherSuggestion OtherSuggestion { get; set; } = default!;

    [ForeignKey("UserId")]
    public UserList UserList { get; set; } = default!;
    public string UserId { get; set; } = default!;
}
