using CookingHelper.LineDto;

namespace CookingHelper.LineDto;

public class BaseMessageObject
{
    public string Type { get; set; } = default!;

    public QuickReplyItemDto? QuickReply { get; set; }

    public class QuickReplyItemDto
    {
        public List<QuickReplyButtonDto> Items { get; set; } = default!;
    }

    public class QuickReplyButtonDto
    {
        public string Type { get; set; } = "action";
        public string? ImageUrl { get; set; }
        public ActionDto Action { get; set; } = default!;
    }
}
