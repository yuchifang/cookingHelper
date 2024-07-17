using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

abstract class StorageInputStatus
{
    public abstract void Init();

    public List<object> GetAdditionConfirmHint(InputStorageInfo InputStorageInfoStatic)
    {
        var NameField = FieldFlexComponent(
            StorageManagementKeywordGroup.Name,
            InputStorageInfoStatic.Name
        );
        var AmountField = FieldFlexComponent(
            StorageManagementKeywordGroup.Amount,
            InputStorageInfoStatic.Amount
        );
        var LocationField = FieldFlexComponent(
            StorageManagementKeywordGroup.Location,
            InputStorageInfoStatic.Location
        );
        FlexComponent? PurchaseDateField;
        if (InputStorageInfoStatic.PurchaseDate != null)
        {
            string customFormat = "yyyy-MM-dd";
            string PurchaseDateString = InputStorageInfoStatic
                .PurchaseDate.Value.ToDateTime(new TimeOnly(0, 0))
                .ToString(customFormat);

            PurchaseDateField = FieldFlexComponent(
                StorageManagementKeywordGroup.PurchaseDate,
                PurchaseDateString
            );
        }
        else
        {
            PurchaseDateField = null;
        }
        FlexComponent? ExpiryDateField;
        if (InputStorageInfoStatic.ExpiryDate != null)
        {
            string customFormat = "yyyy-MM-dd";
            string ExpiryDateString = InputStorageInfoStatic
                .ExpiryDate.Value.ToDateTime(new TimeOnly(0, 0))
                .ToString(customFormat);

            ExpiryDateField = FieldFlexComponent(
                StorageManagementKeywordGroup.ExpiryDate,
                ExpiryDateString
            );
        }
        else
        {
            ExpiryDateField = null;
        }

        List<FlexComponent> FieldTable = new List<FlexComponent> { };

        if (NameField != null)
            FieldTable.Add(NameField);
        if (AmountField != null)
            FieldTable.Add(AmountField);
        if (LocationField != null)
            FieldTable.Add(LocationField);
        if (PurchaseDateField != null)
            FieldTable.Add(PurchaseDateField);
        if (ExpiryDateField != null)
            FieldTable.Add(ExpiryDateField);
        return new List<object>
        {
            GetBubbleFlexMessageObject(InputStorageInfoStatic.Place, FieldTable)
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
                        GetCancelAdditionQuickReplyButton(),
                        GetSkipQuickReplyButton(),
                        GetAdditionCompleteQuickReplyButton()
                    }
                }
            }
        };
    }

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

    protected QuickReplyButtonDto GetCancelAdditionQuickReplyButton()
    {
        return new QuickReplyButtonDto
        {
            Action = new ActionDto
            {
                Type = ActionTypeEnum.Message,
                Label = "取消新增",
                Text = "取消新增",
            }
        };
    }

    protected QuickReplyButtonDto GetSkipQuickReplyButton()
    {
        return new QuickReplyButtonDto
        {
            Action = new ActionDto
            {
                Type = ActionTypeEnum.Message,
                Label = "略過",
                Text = "略過",
            }
        };
    }

    protected QuickReplyButtonDto GetAdditionCompleteQuickReplyButton()
    {
        return new QuickReplyButtonDto
        {
            Action = new ActionDto
            {
                Type = ActionTypeEnum.Message,
                Label = "新增完成",
                Text = "新增完成",
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
                    }
                }
            };
        }
        else
        {
            return null;
        }
    }
}
