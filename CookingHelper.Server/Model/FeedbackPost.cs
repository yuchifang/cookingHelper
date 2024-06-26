using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingHelper.Model;

public class FeedbackPost
{
    [Key]
    public int Id { get; set; } = default!;
    public DateTime PostDate { get; set; } = default!;
    public string? Text { get; set; }

    [ForeignKey("QuestionReplyId")]
    public int QuestionReplyId { get; set; } = default!;
    public QuestionReply QuestionReply { get; set; } = default!;

    // [ForeignKey("OtherSuggestionId")]
    // public int OtherSuggestionId { get; set; } = default!;
    // public OtherSuggestion OtherSuggestion { get; set; } = default!;

    [ForeignKey("SystemSuggestionId")]
    public int SystemSuggestionId { get; set; } = default!;
    public SystemSuggestion SystemSuggestion { get; set; } = default!;
}
