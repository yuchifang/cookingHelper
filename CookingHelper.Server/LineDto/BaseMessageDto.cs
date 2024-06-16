using CookingHelper.LineDto;


namespace CookingHelper.LineDto;

public class BaseMessageDto
{
    public string Type { get; set; }
    // Quick reply
    public QuickReplyItemDto QuickReply { get; set; }

    public class QuickReplyItemDto
    {
        public List<QuickReplyButtonDto> Items { get; set; }
    }

    public class QuickReplyButtonDto
    {
        public string Type { get; set; } = "action";
        public string? ImageUrl { get; set; }
        public ActionDto Action { get; set; }
    }
    // Sender
    public SenderDto? Sender { get; set; }

}
public class SenderDto
{
    public string? Name { get; set; }
    public string? IconUrl { get; set; }
}