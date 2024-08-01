using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

public class MessageUI
{
    public QuickReplyItemDto GetPrevAndNextPageQuickItem()
    {
        return new QuickReplyItemDto
        {
            Items = new List<QuickReplyButtonDto>
            {
                GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
            }
        };
    }

    public QuickReplyItemDto GetNextPageQuickItem()
    {
        return new QuickReplyItemDto
        {
            Items = new List<QuickReplyButtonDto>
            {
                GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
            }
        };
    }

    public QuickReplyItemDto GetPrevPageQuickItem()
    {
        return new QuickReplyItemDto
        {
            Items = new List<QuickReplyButtonDto>
            {
                GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
            }
        };
    }

    public List<object> DateTypeErrorHint(string HintText)
    {
        return new List<object>
        {
            new TextMessageObject
            {
                Text = HintText,
                Emojis = new List<TextMessageEmojiDto>
                {
                    new TextMessageEmojiDto
                    {
                        Index = 0,
                        ProductId = "5ac21ae3040ab15980c9b440",
                        EmojiId = "067"
                    }
                },
                QuickReply = new QuickReplyItemDto
                {
                    Items = new List<QuickReplyButtonDto>
                    {
                        GetQuickReplyButton(ActionTypeEnum.Message, "填寫完成", "填寫完成"),
                        GetQuickReplyButton(ActionTypeEnum.Message, "略過", "略過"),
                        GetQuickReplyButton(ActionTypeEnum.Message, "取消新增", "取消新增")
                    }
                }
            }
        };
    }

    public FlexComponent? FieldFlexComponent(string? keyString, string? valueString)
    {
        if (valueString != null && valueString != "")
        {
            return new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Horizontal,
                Contents = new List<FlexComponent>
                {
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Text,
                        Text = keyString,
                        Size = "sm",
                        Color = "#555555",
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Text,
                        Text = valueString,
                        Size = "sm",
                        Color = "#111111",
                        Align = "end"
                    },
                }
            };
        }
        else
        {
            return null;
        }
    }

    public QuickReplyButtonDto GetQuickReplyButton(string Type, string Text, string Label)
    {
        return new QuickReplyButtonDto
        {
            Action = new ActionDto
            {
                Type = Type,
                Label = Label,
                Text = Text,
            }
        };
    }
}
