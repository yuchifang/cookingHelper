using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

class MessageUI
{
    public FlexMessageObject<FlexBubbleContainer> GetBubbleFlexMessageObject(
        string placeValueText,
        List<FlexComponent> FieldTable
    )
    {
        return new FlexMessageObject<FlexBubbleContainer>
        {
            AltText = "Display Temporary Input",
            Contents = new FlexBubbleContainer
            {
                Type = FlexContainerTypeEnum.Bubble,
                Styles = new FlexBubbleContainerStyle
                {
                    Footer = new FlexBlockStyle { Separator = false }
                },
                Body = new FlexComponent
                {
                    Type = FlexComponentTypeEnum.Box,
                    Layout = FlexComponentLayoutTypeEnum.Vertical,

                    Contents = new List<FlexComponent>
                    {
                        new FlexComponent
                        {
                            Type = FlexComponentTypeEnum.Box,
                            Layout = FlexComponentLayoutTypeEnum.Horizontal,
                            AlignItems = "center",
                            Contents = new List<FlexComponent>
                            {
                                new FlexComponent
                                {
                                    Type = FlexComponentTypeEnum.Text,
                                    Text = StorageManagementKeywordGroup.Place,
                                    Size = "xs",
                                },
                                new FlexComponent
                                {
                                    Type = FlexComponentTypeEnum.Text,
                                    Text = placeValueText,
                                    Size = "xl",
                                    Align = "end"
                                }
                            }
                        },
                        new FlexComponent
                        {
                            Type = FlexComponentTypeEnum.Box,
                            Layout = FlexComponentLayoutTypeEnum.Vertical,
                            Margin = "xxl",
                            Spacing = "sm",
                            Contents = FieldTable
                        },
                        new FlexComponent
                        {
                            Type = FlexComponentTypeEnum.Separator,
                            Margin = "xxl"
                        },
                        new FlexComponent
                        {
                            Type = FlexComponentTypeEnum.Box,
                            Layout = FlexComponentLayoutTypeEnum.Vertical,
                            Contents = new List<FlexComponent>
                            {
                                new FlexComponent
                                {
                                    Type = FlexComponentTypeEnum.Button,
                                    Action = new ActionDto
                                    {
                                        Type = ActionTypeEnum.Message,
                                        Label = "新增",
                                        Text = "新增"
                                    }
                                },
                                new FlexComponent
                                {
                                    Type = FlexComponentTypeEnum.Button,
                                    Action = new ActionDto
                                    {
                                        Type = ActionTypeEnum.Postback,
                                        Label = "修改",
                                        Data = "修改",
                                        InputOption = PostbackInputOptionEnum.OpenKeyboard
                                    }
                                },
                                new FlexComponent
                                {
                                    Type = FlexComponentTypeEnum.Button,
                                    Action = new ActionDto
                                    {
                                        Type = ActionTypeEnum.Message,
                                        Label = "取消新增",
                                        Text = "取消新增",
                                    }
                                }
                            }
                        }
                    }
                }
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
                        GetQuickReplyButton(ActionTypeEnum.Message, "新增完成", "新增完成"),
                        GetQuickReplyButton(ActionTypeEnum.Message, "略過", "略過"),
                        GetQuickReplyButton(ActionTypeEnum.Message, "取消新增", "取消新增")
                    }
                }
            }
        };
    }

    protected FlexComponent? FieldFlexComponent(string? keyString, string? valueString)
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

    protected QuickReplyButtonDto GetQuickReplyButton(string Type, string Text, string Label)
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
