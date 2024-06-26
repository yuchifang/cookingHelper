using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingHelper.Model;

public class QuestionReply
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int QuestionReplyId { get; set; } = default!;

    [ForeignKey("FeedbackGroupId")]
    public int FeedbackGroupId { get; set; } = default!;
    public FeedbackGroup FeedbackGroup { get; set; } = default!;

    public ICollection<FeedbackPost> PostList { get; set; } = new List<FeedbackPost>();
}
