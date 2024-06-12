namespace CookingHelper.LineDto;

public class WebhookEventDto
{
    // -------- 以下 common property --------
    public string Type { get; set; } = default!; // 事件類型
    public string Mode { get; set; } = default!; // Channel state : active | standby
    public long Timestamp { get; set; } // 事件發生時間 : event occurred time in milliseconds
    public SourceDto? Source { get; set; } // 事件來源 : user | group chat | multi-person chat
    public string WebhookEventId { get; set; } = default!; // webhook event id - ULID format
    public DeliverycontextDto DeliveryContext { get; set; } = default!; // 是否為重新傳送之事件 DeliveryContext.IsRedelivery : true | false


    public string? ReplyToken { get; set; } // 回覆此事件所使用的 token
    public MessageEventDto? Message { get; set; } // 收到訊息的事件，可收到 text、sticker、image、file、video、audio、location 訊息
}
// -------- 以下 common property --------
// 可以表示群組,社群,個人的訊息
public class SourceDto
{
    public string Type { get; set; } = default!;
    public string? UserId { get; set; }
    public string? GroupId { get; set; }
    public string? RoomId { get; set; }
}
public class DeliverycontextDto
{
    public bool IsRedelivery { get; set; }

}
public class MessageEventDto
{
    public string Id { get; set; } = default!;
    public string Type { get; set; } = default!;

    // Text Message Event
    public string? Text { get; set; }
    public List<TextMessageEventEmojiDto>? Emojis { get; set; }
    public TextMessageEventMentionDto? Mention { get; set; }
}
public class TextMessageEventEmojiDto
{
    public int Index { get; set; }
    public int Length { get; set; }
    public string ProductId { get; set; } = default!;
    public string EmojiId { get; set; } = default!;
}

public class TextMessageEventMentionDto
{
    public List<TextMessageEventMentioneeDto>? Mentionees { get; set; }
}

public class TextMessageEventMentioneeDto
{
    public int Index { get; set; }
    public int Length { get; set; }
    public string? UserId { get; set; }
}