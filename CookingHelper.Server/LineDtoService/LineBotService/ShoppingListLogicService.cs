using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageEventObject;

namespace CookingHelper.LineDtoService;

public class ShoppingListLogicService
{
    private readonly UserListDatabaseService _shoppingListDatabaseService;

    public ShoppingListLogicService(UserListDatabaseService ShoppingListDatabaseService)
    {
        _shoppingListDatabaseService = ShoppingListDatabaseService;
    }

    public async Task<LineBotWebhookServiceReturnType<TextMessageEventObject>> UpdateShoppingList(
        WebhookEventDto WebHookEventDto,
        string _WebhookEventState
    )
    {
        /*
        依據 _WebhookEventState及 WebHookEventDto.Message!.Text判斷是否
        是直接輸入
        */
        var ReplyMessageList = new List<TextMessageEventObject>();
        // 使用者選擇 PurchaseList 又選 Feedback, MenuList,StorageManagement 情況
        //? 不確定要不要檔
        if (
            WebHookEventDto.Message!.Text == KeywordGroup.Feedback
            || WebHookEventDto.Message!.Text == KeywordGroup.MenuList
            || WebHookEventDto.Message!.Text == KeywordGroup.StorageManagement
        )
        {
            ReplyMessageList.Add(new TextMessageEventObject { Text = "無法記錄此字串, 請重新輸入", });
            return new LineBotWebhookServiceReturnType<TextMessageEventObject>
            {
                replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = ReplyMessageList
                },
                WebhookEventState = _WebhookEventState,
            };
        }

        var UserData = await _shoppingListDatabaseService.GetUserData(
            WebHookEventDto.Source!.UserId!
        );

        if (
            _WebhookEventState == KeywordGroup.InputPurchaseList
            && WebHookEventDto.Message!.Text != KeywordGroup.PurchaseList
        )
        {
            await _shoppingListDatabaseService.UpdateUserShoppingText(
                null,
                WebHookEventDto.Message!.Text!,
                UserData
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
            return new LineBotWebhookServiceReturnType<TextMessageEventObject>
            {
                replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = ReplyMessageList
                },
                WebhookEventState = _WebhookEventState,
            };
        }

        if (UserData.ShoppingListText == "")
        {
            ReplyMessageList.Add(
                new TextMessageEventObject { Text = "沒有物品在採買清單, 開啟輸入框, 輸入想要紀錄的物品", }
            );
            _WebhookEventState = KeywordGroup.InputPurchaseList;
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
                                    Label = "將採買清單帶入輸入框",
                                    Data = "quick reply postback action",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                    FillInText = UserData.ShoppingListText!
                                }
                            },
                        }
                    }
                }
            );
            _WebhookEventState = KeywordGroup.InputPurchaseList;
        }
        return new LineBotWebhookServiceReturnType<TextMessageEventObject>
        {
            WebhookEventState = _WebhookEventState,
            replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
            {
                ReplyToken = WebHookEventDto.ReplyToken!,
                Messages = ReplyMessageList
            },
        };
    }
}
