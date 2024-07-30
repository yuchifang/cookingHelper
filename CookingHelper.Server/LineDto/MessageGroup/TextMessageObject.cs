using CookingHelper.Enum;
using CookingHelper.LineDto;

namespace CookingHelper.LineDto;

public class TextMessageObject : BaseMessageObject
{
    public TextMessageObject()
    {
        Type = MessageTypeEnum.Text;
    }

    public string Text { get; set; }

    public List<TextMessageEmojiDto>? Emojis { get; set; }
}

public class TextMessageEmojiDto
{
    public int? Index { get; set; }
    public string? ProductId { get; set; }
    public string? EmojiId { get; set; }
}
