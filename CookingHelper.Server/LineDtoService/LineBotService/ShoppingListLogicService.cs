using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageEventObject;

namespace CookingHelper.LineDtoService;

public class ShoppingListLogicService
{
    private readonly ShoppingListDatabaseService _shoppingListDatabaseService;

    public ShoppingListLogicService(ShoppingListDatabaseService ShoppingListDatabaseService)
    {
        _shoppingListDatabaseService = ShoppingListDatabaseService;
    }

    //  從資料庫取使用者資料, 判斷是否有值, 並依資料產生對應的輔助按鈕, 且一定要依輔助按鈕操作, 直接輸入會發生錯誤
    // _WebhookEventState 有兩種狀態 輸入狀態PurchaseListInput, 顯示狀態PurchaseList
    public async Task<ShoppingListLogicServiceReturnType<TextMessageEventObject>> Init(
        WebhookEventDto WebHookEventDto,
        string _WebhookEventState
    )
    {
        var UserData = await _shoppingListDatabaseService.GetUserData(
            WebHookEventDto.Source!.UserId!
        );

        List<TextMessageEventObject>? ReplyMessageList = new List<TextMessageEventObject>();

        if (_WebhookEventState == KeywordGroup.PurchaseListInput)
        {
            await _shoppingListDatabaseService.UpdateUserShoppingText(
                WebHookEventDto.Source.UserId!,
                WebHookEventDto.Message!.Text!,
                null
            );

            ReplyMessageList.Add(
                new TextMessageEventObject
                {
                    Text = "更新完成",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Postback,
                                    Label = "返回主目錄",
                                    Text = "返回主目錄",
                                    Data = "quick reply postback action",
                                    InputOption = PostbackInputOptionEnum.OpenRichMenu,
                                }
                            },
                        }
                    }
                }
            );
            _WebhookEventState = "";
            return new ShoppingListLogicServiceReturnType<TextMessageEventObject>
            {
                replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = ReplyMessageList
                },
                WebhookEventState = _WebhookEventState,
            };
        }

        if (WebHookEventDto.Message!.Text == "採買清單已清空,可直接輸入")
        {
            await _shoppingListDatabaseService.UpdateUserShoppingText(null, "", UserData);
            _WebhookEventState = KeywordGroup.PurchaseListInput;
            return new ShoppingListLogicServiceReturnType<TextMessageEventObject>
            {
                replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = ReplyMessageList
                },
                WebhookEventState = _WebhookEventState,
            };
        }

        if (WebHookEventDto.Message!.Text == "將採買清單帶入輸入框")
        {
            _WebhookEventState = KeywordGroup.PurchaseListInput;
            return new ShoppingListLogicServiceReturnType<TextMessageEventObject>
            {
                replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = ReplyMessageList
                },
                WebhookEventState = _WebhookEventState,
            };
        }

        if (WebHookEventDto.Message!.Text == "開啟輸入框")
        {
            _WebhookEventState = KeywordGroup.PurchaseListInput;
            return new ShoppingListLogicServiceReturnType<TextMessageEventObject>
            {
                replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = ReplyMessageList
                },
                WebhookEventState = _WebhookEventState,
            };
        }

        if (
            _WebhookEventState == KeywordGroup.PurchaseList
            && WebHookEventDto.Message!.Text != KeywordGroup.PurchaseList
        )
        {
            ReplyMessageList.AddRange(
                [
                    new TextMessageEventObject { Text = "無法直接輸入,請依按鈕操作", },
                    new TextMessageEventObject { Text = "採買清單", }
                ]
            );
        }

        if (UserData.ShoppingListText == "" && _WebhookEventState == KeywordGroup.PurchaseList)
        {
            ReplyMessageList.Add(
                new TextMessageEventObject
                {
                    Text = "沒有物品在採買清單, 開啟輸入框, 輸入想要紀錄的物品",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Postback,
                                    Label = "開啟輸入框",

                                    Data = "quick reply postback action",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                }
                            },
                        }
                    }
                }
            );
            _WebhookEventState = KeywordGroup.PurchaseListInput;
        }
        else
        {
            ReplyMessageList.Add(
                new TextMessageEventObject
                {
                    Text = UserData.ShoppingListText!,
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Postback,
                                    Label = "清空採買清單",
                                    Text = "採買清單已清空,可直接輸入",
                                    Data = "quick reply postback action",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Postback,
                                    Label = "將採買清單帶入輸入框",
                                    Text = "將採買清單帶入輸入框",
                                    Data = "quick reply postback action",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                    FillInText = UserData.ShoppingListText!
                                }
                            },
                        }
                    }
                }
            );
        }

        return new ShoppingListLogicServiceReturnType<TextMessageEventObject>
        {
            replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
            {
                ReplyToken = WebHookEventDto.ReplyToken!,
                Messages = ReplyMessageList
            },
            WebhookEventState = _WebhookEventState,
        };
    }
}

public class ShoppingListLogicServiceReturnType<T>
{
    public ReplyMessageRequestDto<T> replyMessageRequest { get; set; } = default!;
    public string WebhookEventState { get; set; } = default!;
}
