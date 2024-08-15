using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

public class ShoppingListService
{
    private readonly ShoppingListDatabaseService _shoppingListDatabaseService;

    public ShoppingListService(ShoppingListDatabaseService ShoppingListDatabaseService)
    {
        _shoppingListDatabaseService = ShoppingListDatabaseService;
    }

    public async Task Init(WebhookEventDto WebHookEventDto)
    {
        string WebHookEventMessage = WebHookEventDto.Message!.Text!;
        /*
        依據 _WebhookEventState及 WebHookEventDto.Message!.Text判斷是否
        是直接輸入
        */
        var ReplyMessageList = new List<TextMessageObject>();

        // 使用者選擇 PurchaseList 又選 Feedback, MenuList,StorageManagement 情況
        if (
            WebHookEventMessage == KeywordGroup.Feedback
            || WebHookEventMessage == KeywordGroup.MenuList
            || WebHookEventMessage == KeywordGroup.StorageManagement
        )
        {
            ReplyMessageList.Add(new TextMessageObject { Text = "無法記錄此字串, 請重新輸入", });

            LineBotService._ReplyMessageRequestStatic =
                new ReplyMessageRequestDto<TextMessageObject>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = ReplyMessageList
                };
            return;
        }

        var UserList = await _shoppingListDatabaseService.GetUserList(
            WebHookEventDto.Source!.UserId!
        );

        if (
            LineBotService._WebhookEventStatusStatic == KeywordGroup.InputPurchaseList
            && WebHookEventMessage != KeywordGroup.PurchaseList
        )
        {
            await _shoppingListDatabaseService.UpdateUserShoppingText(
                null,
                WebHookEventMessage,
                UserList
            );

            ReplyMessageList.Add(
                new TextMessageObject
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
            LineBotService._WebhookEventStatusStatic = "";
            LineBotService._ReplyMessageRequestStatic =
                new ReplyMessageRequestDto<TextMessageObject>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = ReplyMessageList
                };
            return;
        }

        if (UserList.ShoppingListText == "")
        {
            ReplyMessageList.Add(new TextMessageObject { Text = "採買清單中沒有物品, 開啟輸入框, 輸入想要紀錄的物品", });
            LineBotService._WebhookEventStatusStatic = KeywordGroup.InputPurchaseList;
        }
        else
        {
            ReplyMessageList.AddRange(
                [
                    new TextMessageObject { Text = "開啟輸入框, 輸入要記錄的物品", },
                    new TextMessageObject
                    {
                        Text = "採買清單: " + UserList.ShoppingListText!,
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                new QuickReplyButtonDto
                                {
                                    Action = new ActionDto
                                    {
                                        Type = ActionTypeEnum.Postback,
                                        Label = "將上次的採買清單帶入輸入框",
                                        Data = "quick reply postback action",
                                        InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                        FillInText = UserList.ShoppingListText!
                                    }
                                },
                            }
                        }
                    }
                ]
            );
            LineBotService._WebhookEventStatusStatic = KeywordGroup.InputPurchaseList;
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<TextMessageObject>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = ReplyMessageList
        };
    }
}
