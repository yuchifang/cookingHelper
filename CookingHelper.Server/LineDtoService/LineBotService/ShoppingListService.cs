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
        string WebHookEventMessage;
        if (
            WebHookEventDto.GetType().GetProperty("Postback") != null
            && WebHookEventDto.Postback != null
        )
        {
            WebHookEventMessage = WebHookEventDto.Postback!.Data!;
        }
        else
        {
            WebHookEventMessage = WebHookEventDto.Message!.Text!;
        }

        var ReplyMessageList = new List<TextMessageObject>();

        var UserList = await _shoppingListDatabaseService.GetUserList(
            WebHookEventDto.Source!.UserId!
        );

        if (
            LineBotService._WebhookEventStatusStatic == KeywordGroup.InputPurchaseList
            && WebHookEventMessage != KeywordGroup.PurchaseList
        )
        {
            LineBotService._WebhookEventStatusStatic = "";

            await _shoppingListDatabaseService.UpdateUserShoppingText(
                null,
                WebHookEventMessage,
                UserList
            );

            await Init(WebHookEventDto);
        }

        if (UserList.ShoppingListText == "")
        {
            LineBotService._WebhookEventStatusStatic = KeywordGroup.InputPurchaseList;
            ReplyMessageList.Add(new TextMessageObject { Text = "採買清單中沒有物品, 開啟輸入框, 輸入想要紀錄的物品", });
        }
        else
        {
            LineBotService._WebhookEventStatusStatic = KeywordGroup.InputPurchaseList;
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
                                        Label = "將採買清單帶入輸入框",
                                        Data = "quick reply postback action",
                                        InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                        FillInText = UserList.ShoppingListText!
                                    }
                                },
                                new QuickReplyButtonDto
                                {
                                    Action = new ActionDto
                                    {
                                        Type = ActionTypeEnum.Postback,
                                        Label = "清空採買清單",
                                        Data = "清空採買清單",
                                        InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                    }
                                },
                            }
                        }
                    }
                ]
            );
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<TextMessageObject>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = ReplyMessageList
        };
    }
}
